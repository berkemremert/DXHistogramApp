using DevExpress.Charts.Designer;
using DevExpress.Utils;
using DevExpress.Xpf.Charts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace DXHistogram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<double> currentData;

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

        private void GenerateSampleData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtSampleCount.Text, out int count) || count <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for count.", "Invalid Count", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(txtSampleMean.Text, out double mean))
                {
                    MessageBox.Show("Please enter a valid number for mean.", "Invalid Mean", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(txtSampleStdDev.Text, out double stdDev) || stdDev <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for standard deviation.", "Invalid Std Dev", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                currentData = GenerateSampleData(count, mean, stdDev);
                UpdateHistogram();
                MessageBox.Show($"Generated {count} sample data points.", "Sample Data Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating sample data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateHistogram_Click(object sender, RoutedEventArgs e)
        {
            UpdateHistogram();
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

                if (!int.TryParse(txtBinCount.Text, out int binCount) || binCount <= 0)
                {
                    MessageBox.Show("Please enter a valid positive number for bin count.", "Invalid Bin Count", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var histogramData = CreateHistogramBins(currentData, binCount);
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
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Select Data File"
                };

                if (openFileDialog.ShowDialog().Equals("OK"))
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
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "XML files (*.xml)|*.xml",
                    Title = "Save Chart Layout",
                    DefaultExt = "xml",
                    FileName = "ChartLayout.xml"
                };

                if (saveFileDialog.ShowDialog().Equals("OK"))
                {
                    // Save the chart layout to XML
                    chartControl.ExportToXlsx(saveFileDialog.FileName);
                    MessageBox.Show($"Chart layout saved successfully to {saveFileDialog.FileName}",
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
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "XML files (*.xml)|*.xml",
                    Title = "Load Chart Layout"
                };

                if (openFileDialog.ShowDialog().Equals("OK"))
                {
                    // Load the chart layout from XML
                    //TODO:
                    //chartControl.RestoreLayoutFromXml(openFileDialog.FileName);
                    MessageBox.Show($"Chart layout loaded successfully from {openFileDialog.FileName}",
                        "Layout Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chart layout: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv",
                    Title = "Save Data File",
                    DefaultExt = "txt"
                };

                if (saveFileDialog.ShowDialog().Equals("OK"))
                {
                    string content = string.Join(Environment.NewLine, currentData.Select(x => x.ToString(CultureInfo.InvariantCulture)));
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