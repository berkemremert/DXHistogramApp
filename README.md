# DXHistogram - DevExpress WPF Histogram Visualization Tool

## Features

### Data Input & Management
- **Manual Data Entry**: Enter values directly in a text area (supports comma, space, tab, and newline separators)
- **File Import**: Load data from TXT or CSV files with automatic parsing
- **File Export**: Save current dataset to TXT or CSV files with proper formatting
- **Data Validation**: Real-time validation of input data with error reporting
- **Clear Functionality**: Quick data clearing and reset

### Interactive Histogram Visualization
- **Automatic Binning**: Intelligent histogram bin creation (default 10 bins)
- **Real-time Updates**: Histogram updates automatically when data changes
- **Professional Styling**: Clean, modern chart appearance using DevExpress themes

### Chart Customization
- **Chart Designer**: Full-featured DevExpress Chart Designer integration for advanced customization
- **Layout Persistence**: Save and load custom chart layouts as XML files
- **Dynamic Binding**: Automatic data binding with proper reconnection after layout changes

### Statistical Analysis
- **Real-time Statistics**: Live calculation of key statistical measures:
  - Count of data points
  - Mean (average)
  - Standard deviation
  - Minimum value
  - Maximum value

### Sample Data Generation
- **Normal Distribution**: Built-in generator creates sample data with configurable mean and standard deviation
- **Box-Muller Transform**: Uses proper statistical methods for realistic data generation

## 🛠️ Technology Stack

### Core Framework
- **WPF (Windows Presentation Foundation)**: Main application framework
- **C# (.NET Framework/Core)**: Primary programming language
- **XAML**: User interface markup

### DevExpress Components
- **DevExpress.Xpf.Charts**: Core charting functionality
  - `ChartControl`: Main chart container
  - `XYDiagram2D`: 2D coordinate system
  - `BarSideBySideSeries2D`: Histogram bar series
  - `AxisX2D` & `AxisY2D`: Chart axes with titles
- **DevExpress.Charts.Designer**: Chart customization designer
  - `ChartDesigner`: Visual chart editor
- **DevExpress.Xpf.Dialogs**: Consistent file dialogs
  - `DXSaveFileDialog`: DevExpress save file dialog
  - `DXOpenFileDialog`: DevExpress open file dialog
- **DevExpress.Utils**: Utility functions and helpers

### .NET Components
- **System.Windows**: WPF window and UI controls
- **System.IO**: File operations and management
- **System.Globalization**: Number parsing and formatting with culture support
- **System.Linq**: Data querying and manipulation
- **System.Collections.Generic**: Generic collections (List<T>)
- **Microsoft.Win32**: Windows registry access (if needed)

### UI Components Used
- **Grid**: Main layout container with column definitions
- **StackPanel**: Vertical stacking of UI elements
- **GroupBox**: Logical grouping of related controls
- **TextBox**: Data input area with multi-line support
- **Button**: Action triggers for various operations
- **TextBlock**: Display of labels and statistics
- **Border**: Visual styling container
- **ScrollViewer**: Scrollable content areas
- **Separator**: Visual dividers between sections

## 📋 System Requirements

- Windows 10/11 or Windows Server 2016+
- .NET Framework 4.7.2+ or .NET 5+
- DevExpress WPF Controls (requires license)
- Visual Studio 2019+ (for development)

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone [repository-url]
   cd DXHistogram
   ```

2. **Install DevExpress Components**
   - Install DevExpress Universal subscription or trial
   - Ensure the following packages are available:
     - DevExpress.Wpf.Charts
     - DevExpress.Wpf.Core
     - DevExpress.Charts.Core

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

## Usage Guide

### Loading Data
1. **Manual Entry**: Type or paste values into the text area, separated by commas, spaces, or newlines
2. **From File**: Click "Load from File" to import data from TXT/CSV files
3. **Sample Data**: The application loads sample normal distribution data by default

### Viewing Statistics
- Real-time statistics are displayed in the "Data Statistics" panel
- Includes count, mean, standard deviation, minimum, and maximum values

### Customizing Charts
1. Click "Open Chart Designer" to access the full DevExpress chart editor
2. Modify colors, fonts, axes, legends, and other visual properties
3. Changes are applied automatically

### Saving & Loading Layouts
- **Save Layout**: Export current chart configuration to XML file
- **Load Layout**: Import previously saved chart configuration
- Layouts preserve all visual customizations

### Data Export
- Click "Save Data to File" to export current dataset
- Choose between TXT (plain values) or CSV (with headers) format

## Architecture Overview

### Core Classes
- **MainWindow**: Main application window and primary controller
- **HistogramBin**: Data model representing histogram bins with range and frequency

### Key Methods
- **CreateHistogramBins()**: Converts raw data into histogram bins
- **GenerateSampleData()**: Creates sample normal distribution data
- **UpdateStatistics()**: Calculates and displays statistical measures
- **CreateBindings()**: Reconnects data binding after layout changes

### Data Flow
1. Data Input → Validation → Storage in `currentData` List
2. Data Processing → Histogram Bin Creation
3. UI Update → Chart Rendering → Statistics Calculation

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## DevExpress License Notice

This application requires DevExpress WPF controls which are commercial components. You need a valid DevExpress license to build and distribute this application. DevExpress offers trial versions for evaluation purposes.

## Known Issues & Troubleshooting

### Common Issues
1. **Chart Designer not opening**: Ensure DevExpress.Charts.Designer is properly referenced
2. **Data binding lost after layout load**: The `CreateBindings()` method should automatically handle this
3. **File parsing errors**: Check that data files contain only numeric values

### Performance Considerations
- Large datasets (>10,000 points) may experience slower histogram generation
- Consider implementing data sampling for very large datasets

## Support

For issues related to:
- **DevExpress Components**: Contact DevExpress Support
- **Application Logic**: Create an issue in this repository
- **Feature Requests**: Open a discussion in the repository
