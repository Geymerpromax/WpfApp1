using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private PlotModel plotModel;

        public MainWindow()
        {
            InitializeComponent();
            CreateFunctionGraf();
            functionComboBox.SelectedIndex = 1; // Default to y = sin x

        }

        private void DrawBoundaries_Click(object sender, RoutedEventArgs e)
        {
            // Проверка ввода значений для левой и правой границ
            if (!double.TryParse(leftBoundaryTextBox.Text, out double leftBoundary) ||
                !double.TryParse(rightBoundaryTextBox.Text, out double rightBoundary))
            {
                infoTextBox.Text += "Ошибка ввода границ. Пожалуйста, введите числовые значения.\n";
                return;
            }

            // Проверка, что левая граница строго меньше правой
            if (leftBoundary >= rightBoundary)
            {
                infoTextBox.Text += "Левая граница должна быть строго меньше правой.\n";
                return;
            }

            // Очистка только границ
            ClearBoundaryLines();

            // Добавление границ
            DrawVerticalLine(leftBoundary);
            DrawVerticalLine(rightBoundary);

            // Обновление информационного поля
            infoTextBox.Text += $"Отрисованы границы x = {leftBoundary} и x = {rightBoundary}.\n";
        }

        private void CreateGraf(object sender, RoutedEventArgs e)
        {
            CreateFunctionGraf();
        }
        private void CreateFunctionGraf()
        {
            plotModel = new PlotModel { Title = "Sin(x)" };

            // Настройка оси X
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = -360,
                Maximum = 360,
                Title = "Angle (degrees)",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };

            // Настройка оси Y
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -2,
                Maximum = 2,
                Title = "Sin(x)",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };

            // Добавление осей в модель
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Генерация данных для графика синуса
            var sinSeries = new LineSeries
            {
                Title = "Sin(x)",
                Color = OxyColors.Blue,
                StrokeThickness = 2
            };

            for (int i = -129600; i <= 129600; i++)
            {
                sinSeries.Points.Add(new DataPoint(i, Math.Sin(i * Math.PI / 180)));
            }

            // Добавление серии в модель
            plotModel.Series.Add(sinSeries);

            // Выделение точек пересечения осей
            plotModel.Annotations.Add(new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = 0,
                Color = OxyColors.Red,
                StrokeThickness = 2
            });

            plotModel.Annotations.Add(new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Y = 0,
                Color = OxyColors.Red,
                StrokeThickness = 2
            });

            // Присвоение модели графику
            plotView.Model = plotModel;
        }

        private static readonly Regex _regex = new Regex(@"^-?\d*\.?\d*$" + "[^0-9.-]+"); // Регулярное выражение, которое соответствует недопустимым символам

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void NumericTextBoxInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void ClearBoundaryLines()
        {
            // Находим все аннотации, представляющие границы, и удаляем их
            var boundaryAnnotations = plotModel.Annotations.Where(annotation => annotation is LineAnnotation lineAnnotation && lineAnnotation.Text?.StartsWith("x = ") == true).ToList();
            foreach (var annotation in boundaryAnnotations)
            {
                plotModel.Annotations.Remove(annotation);
            }
            plotView.InvalidatePlot(true);
        }

        private void DrawVerticalLine(double xValue)
        {
            var verticalLineAnnotation = new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = xValue,
                Color = OxyColors.Green,
                StrokeThickness = 2,
                Text = $"x = {xValue}",
            };
            plotModel.Annotations.Add(verticalLineAnnotation);
            plotView.InvalidatePlot(true);
        }
        private void CalculateRoots_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(leftBoundaryTextBox.Text, out double leftBoundary) ||
                !double.TryParse(rightBoundaryTextBox.Text, out double rightBoundary) ||
                !double.TryParse(stepTextBox.Text, out double step))
            {
                infoTextBox.Text += "Ошибка ввода значений. Пожалуйста, введите числовые значения для границ и шага.\n";
                return;
            }

            if (leftBoundary >= rightBoundary)
            {
                infoTextBox.Text += "Левая граница должна быть строго меньше правой.\n";
                return;
            }

            if (step <= 0)
            {
                infoTextBox.Text += "Шаг должен быть положительным числом.\n";
                return;
            }
            infoTextBox.Text = "Значения функции:\n";

            double previousValue = CalculateFunction(leftBoundary);
            double previousX = leftBoundary;
            bool firstRootFound = false;

            for (double x = leftBoundary + step; x <= rightBoundary; x += step)
            {
                double currentValue = CalculateFunction(x);

                infoTextBox.Text += $"x = {x:F6}, f(x) = {currentValue:F6}\n";

                if (previousValue * currentValue < 0)
                {
                    if (!firstRootFound)
                    {
                        infoTextBox.Text += $"Корень перехода через 0: [{previousX:F6}, {x:F6}]\n";
                        firstRootFound = true;
                    }
                    else
                    {
                        infoTextBox.Text += $"Корень перехода через 0: [{previousX:F6}, {x:F6}]\n";
                    }
                }

                previousValue = currentValue;
                previousX = x;
            }
        }





        private double CalculateFunction(double x)
        {
            switch ((functionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString())
            {
                case "y = x + 5":
                    return x + 5;
                case "y = sin x":
                    return Math.Sin(x * Math.PI / 180);
                default:
                    throw new InvalidOperationException("Неизвестная функция.");
            }
        }
    }
}
