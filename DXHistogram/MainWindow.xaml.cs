using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Globalization;
using DevExpress.Xpf.Charts;
using DevExpress.Utils;
using System.Xml.Linq;
using MessageBox = System.Windows.MessageBox;
using DevExpress.Charts.Designer;
using DevExpress.Xpf.Dialogs;  // Add this using for DX dialogs

namespace DXHistogram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<double> currentData;

        // Constants for status messages
        const string LayoutSavedFormatString = "The Chart Layout saved to the '{0}' file";
        const string LayoutLoadedFormatString = "The Chart Layout loaded from the '{0}' file";

        public MainWindow()
        {
            InitializeComponent();
            currentData = new List<double>();
            LoadDefaultHistogramData();
        }

        private void LoadDefaultHistogramData()
        {
            // Generate default sample data
            currentData = GenerateSampleData(1000, 50, 15);
            UpdateHistogram();
        }

        private void LoadUserData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = txtDataInput.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    MessageBox.Show("Please enter some data values.", "No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Parse the input data (support both comma and space separated values)
                var dataValues = new List<double>();
                var separators = new char[] { ',', ' ', '\t', '\n', '\r' };
                var tokens = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                foreach (var token in tokens)
                {
                    if (double.TryParse(token.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    {
                        dataValues.Add(value);
                    }
                    else
                    {
                        MessageBox.Show($"Invalid number format: '{token.Trim()}'", "Parse Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (dataValues.Count == 0)
                {
                    MessageBox.Show("No valid numbers found in the input.", "No Valid Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                currentData = dataValues;
                UpdateHistogram();
                MessageBox.Show($"Successfully loaded {dataValues.Count} data points.", "Data Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            txtDataInput.Clear();
            currentData.Clear();
            this.DataContext = new List<HistogramBin>();
            UpdateStatistics();
        }

        private void UpdateHistogram()
        {
            try
            {
                if (currentData == null || !currentData.Any())
                {
                    this.DataContext = new List<HistogramBin>();
                    UpdateStatistics();
                    return;
                }

                // Use default bin count of 10, or let Chart Designer handle bin configuration
                var histogramData = CreateHistogramBins(currentData, 10);
                this.DataContext = histogramData;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating histogram: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use DevExpress Open File Dialog
                DXOpenFileDialog openFileDialog = new DXOpenFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Select Data File",
                    DefaultExt = "txt"
                };

                bool? result = openFileDialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    string content = File.ReadAllText(openFileDialog.FileName);

                    // Parse the file content
                    var dataValues = new List<double>();
                    var separators = new char[] { ',', ' ', '\t', '\n', '\r', ';' };
                    var tokens = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var token in tokens)
                    {
                        if (double.TryParse(token.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                        {
                            dataValues.Add(value);
                        }
                    }

                    if (dataValues.Count > 0)
                    {
                        currentData = dataValues;
                        UpdateHistogram();
                        MessageBox.Show($"Successfully loaded {dataValues.Count} data points from file.", "File Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No valid numbers found in the selected file.", "No Data Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenChartDesigner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the DevExpress Chart Designer using the correct method
                ChartDesigner chartDesigner = new ChartDesigner(chartControl);
                chartDesigner.Show(this);

                // Note: Changes are applied automatically when the designer is used
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening chart designer: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveChartLayout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use DevExpress Save File Dialog as per official documentation
                DXSaveFileDialog dialog = new DXSaveFileDialog
                {
                    DefaultExt = "xml",
                    Filter = "XML files (*.xml)|*.xml",
                    Title = "Save Chart Layout",
                    FileName = "ChartLayout.xml"
                };

                bool? result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    // Save the chart layout to XML
                    chartControl.SaveToFile(dialog.FileName);
                    MessageBox.Show(String.Format(LayoutSavedFormatString, dialog.FileName),
                        "Layout Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving chart layout: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadChartLayout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use DevExpress Open File Dialog as per official documentation
                DXOpenFileDialog dialog = new DXOpenFileDialog
                {
                    DefaultExt = "xml",
                    Filter = "XML files (*.xml)|*.xml",
                    Title = "Load Chart Layout"
                };

                bool? result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    // Load the chart layout from XML
                    chartControl.LoadFromFile(dialog.FileName);

                    // IMPORTANT: LoadFrom... methods create new instances of chart elements to restore the Chart Control's layout.
                    // You should recreate bindings if you bind UI controls to chart elements.
                    CreateBindings();

                    MessageBox.Show(String.Format(LayoutLoadedFormatString, dialog.FileName),
                        "Layout Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chart layout: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Recreates data bindings after loading chart layout
        /// This is required because LoadFromFile creates new chart element instances
        /// </summary>
        private void CreateBindings()
        {
            try
            {
                // Find the bar series (there should be one based on your XAML)
                var barSeries = chartControl.Diagram.Series.OfType<BarSideBySideSeries2D>().FirstOrDefault();

                if (barSeries != null)
                {
                    // Rebind the data source to the current data context
                    barSeries.DataSource = this.DataContext;
                    barSeries.ArgumentDataMember = "Range";
                    barSeries.ValueDataMember = "Frequency";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error recreating bindings: {ex.Message}", "Binding Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentData == null || !currentData.Any())
                {
                    MessageBox.Show("No data to save.", "No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Use DevExpress Save File Dialog
                DXSaveFileDialog saveFileDialog = new DXSaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv",
                    Title = "Save Data File",
                    DefaultExt = "txt",
                    FileName = "HistogramData"
                };

                bool? result = saveFileDialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    string content;
                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();

                    if (extension == ".csv")
                    {
                        // Save as CSV with header
                        content = "Value" + Environment.NewLine +
                                 string.Join(Environment.NewLine, currentData.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    else
                    {
                        // Save as plain text (one value per line)
                        content = string.Join(Environment.NewLine, currentData.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }

                    File.WriteAllText(saveFileDialog.FileName, content);
                    MessageBox.Show($"Data saved successfully to {saveFileDialog.FileName}", "Data Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            if (currentData == null || !currentData.Any())
            {
                lblDataCount.Text = "Count: 0";
                lblDataMean.Text = "Mean: 0";
                lblDataStdDev.Text = "Std Dev: 0";
                lblDataMin.Text = "Min: 0";
                lblDataMax.Text = "Max: 0";
                return;
            }

            var count = currentData.Count;
            var mean = currentData.Average();
            var variance = currentData.Sum(x => Math.Pow(x - mean, 2)) / count;
            var stdDev = Math.Sqrt(variance);
            var min = currentData.Min();
            var max = currentData.Max();

            lblDataCount.Text = $"Count: {count}";
            lblDataMean.Text = $"Mean: {mean:F2}";
            lblDataStdDev.Text = $"Std Dev: {stdDev:F2}";
            lblDataMin.Text = $"Min: {min:F2}";
            lblDataMax.Text = $"Max: {max:F2}";
        }

        private List<double> GenerateSampleData(int count, double mean, double stdDev)
        {
            var random = new Random(DateTime.Now.Millisecond); // Random seed for variety
            var data = new List<double>();

            // Generate sample data points with normal distribution
            for (int i = 0; i < count; i++)
            {
                double value = GenerateNormalDistribution(random, mean, stdDev);
                data.Add(value);
            }

            return data;
        }

        private double GenerateNormalDistribution(Random random, double mean, double stdDev)
        {
            // Box-Muller transformation for normal distribution
            double u1 = 1.0 - random.NextDouble(); // uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        private List<HistogramBin> CreateHistogramBins(List<double> values, int binCount = 10)
        {
            if (!values.Any()) return new List<HistogramBin>();

            var min = values.Min();
            var max = values.Max();

            // Handle case where all values are the same
            if (Math.Abs(max - min) < double.Epsilon)
            {
                return new List<HistogramBin>
                {
                    new HistogramBin
                    {
                        Range = $"{min:F1}",
                        Frequency = values.Count,
                        LowerBound = min,
                        UpperBound = min
                    }
                };
            }

            var binWidth = (max - min) / binCount;
            var bins = new List<HistogramBin>();

            for (int i = 0; i < binCount; i++)
            {
                var lowerBound = min + (i * binWidth);
                var upperBound = min + ((i + 1) * binWidth);

                // For the last bin, include the maximum value
                var count = i == binCount - 1
                    ? values.Count(v => v >= lowerBound && v <= upperBound)
                    : values.Count(v => v >= lowerBound && v < upperBound);

                bins.Add(new HistogramBin
                {
                    Range = $"{lowerBound:F1}-{upperBound:F1}",
                    Frequency = count,
                    LowerBound = lowerBound,
                    UpperBound = upperBound
                });
            }

            return bins;
        }
    }

    // Data model for histogram bins
    public class HistogramBin
    {
        public string Range { get; set; }
        public int Frequency { get; set; }
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
    }
}