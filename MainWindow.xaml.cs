using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;

namespace SVGRecolorTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The file path of the SVG image selected.
        /// </summary>
        private string selectedPath;


        /// <summary>
        /// Instance reference for the svgDocument used and updated throughout the manipulation of the image.
        /// </summary>
        private Svg.SvgDocument svgDocument;


        private HighlightSelectedWordBackgroundRenderer _renderer;

        public MainWindow()
        {
            InitializeComponent();
            txtSvgSource.ShowLineNumbers = true;

            var xmlRsrc = "SVGRecolorTool.rsrc.xmlSyntax.xshd";
            var myAssem = Assembly.GetExecutingAssembly().GetManifestResourceStream(xmlRsrc);
            System.Xml.XmlTextReader reader = new XmlTextReader(myAssem);
            txtSvgSource.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            txtSvgSource.TextArea.Options.EnableHyperlinks = false;

            _renderer = new HighlightSelectedWordBackgroundRenderer(txtSvgSource);
            txtSvgSource.TextArea.TextView.BackgroundRenderers.Add(_renderer);
            txtSvgSource.TextArea.SelectionChanged += ValidateSelection;

        }

        private void ValidateSelection(object sender, EventArgs e) {

            var results = new TextSegmentCollection<SearchResult>();

            if(txtSvgSource.SelectedText.Length > 2) {
                string pattern = @"\b" + txtSvgSource.SelectedText + @"\b";
                Regex rg = new Regex(pattern);
                MatchCollection matchedSelections = rg.Matches(txtSvgSource.Text);

                if (matchedSelections.Count > 1) {
                    foreach (Match result in matchedSelections) {
                        results.Add(new SearchResult(result));
                    }
                } else {
                    results.Clear();
                }
                _renderer.CurrentResults = results;

            } else {
                results.Clear();
                _renderer.CurrentResults = results;
            }

            txtSvgSource.TextArea.TextView.InvalidateLayer(KnownLayer.Selection);

        }

        private void btnBrowse_Click(object sender, EventArgs e) {

            // Configure open file dialog box
            var dialog          = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName     = "Select a SVG"; // Default file name
            dialog.DefaultExt   = ".svg"; // Default file extension
            dialog.Filter       = "SVG Files (.svg)|*.svg"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true) {

                string svgFilename = dialog.FileName;

                var svgDoc = SvgDocument.Open<SvgDocument>(svgFilename, null);

                selectedPath = svgFilename;
                txtSVGFile.Text = svgFilename;
                txtWidth.Text = svgDoc.Width.Value.ToString();
                txtHeight.Text = svgDoc.Height.Value.ToString();
                txtSvgSource.Text = svgDoc.GetXML();

                // var svgMaxSize = new Size(pictConvertedImage.Width, pictConvertedImage.Height);

                var h = (int)pictConvertedImage.Height;

                var bmp = svgDoc.Draw(h, 0);

                var svgBitmap = CreateBitmapSourceFromGdiBitmap(bmp);

                pictConvertedImage.Source = svgBitmap;

            }
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap) {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            } finally {
                bitmap.UnlockBits(bitmapData);
            }
        }

    }
}
