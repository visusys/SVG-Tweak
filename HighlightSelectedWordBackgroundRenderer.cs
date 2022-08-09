using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SVGRecolorTool {

    public class SearchResult : TextSegment {
        /// <summary>The regex match for the search result.</summary>
        public Match Match { get; }

        /// <summary>Constructs the search result from the match.</summary>
        public SearchResult(Match match) {
            this.StartOffset = match.Index;
            this.Length = match.Length;
            this.Match = match;
        }
    }

    public class HighlightSelectedWordBackgroundRenderer : IBackgroundRenderer {

        private TextEditor _editor;

        TextSegmentCollection<SearchResult> currentResults = new TextSegmentCollection<SearchResult>();

        public HighlightSelectedWordBackgroundRenderer(TextEditor editor) {
            _editor = editor;
            Background = new SolidColorBrush(Color.FromArgb(90, 255, 255, 255));
            Background.Freeze();
        }

        public KnownLayer Layer {
            get { return KnownLayer.Caret; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext) {

            if (!textView.VisualLinesValid)
                return;

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
                return;

            int viewStart = visualLines.First().FirstDocumentLine.Offset;
            int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;


            foreach (TextSegment result in currentResults.FindOverlappingSegments(viewStart, viewEnd - viewStart)) {
                BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
                geoBuilder.AlignToWholePixels = true;
                geoBuilder.CornerRadius = 0;
                geoBuilder.AddSegment(textView, result);
                Geometry geometry = geoBuilder.CreateGeometry();
                if (geometry != null) {
                    drawingContext.DrawGeometry(Background, null, geometry);
                }
            }
        }

        public TextSegmentCollection<SearchResult> CurrentResults {
            get { return currentResults; }
            set { currentResults = value; }
        }

        public Brush Background { get; set; }
    }
}
