using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SVGRecolorTool {
    public class MarkSameWord : DocumentColorizingTransformer {
        private readonly string _selectedText;

        public MarkSameWord(string selectedText) {
            _selectedText = selectedText;
        }

        protected override void ColorizeLine(DocumentLine line) {
            if (string.IsNullOrEmpty(_selectedText)) {
                return;
            }

            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;

            while ((index = text.IndexOf(_selectedText, start, StringComparison.Ordinal)) >= 0) {
                ChangeLinePart(
                  lineStartOffset + index, // startOffset
                  lineStartOffset + index + _selectedText.Length, // endOffset
                  element => element.TextRunProperties.SetBackgroundBrush(Brushes.LightSkyBlue));
                start = index + 1; // search for next occurrence
            }
        }
    }
}
