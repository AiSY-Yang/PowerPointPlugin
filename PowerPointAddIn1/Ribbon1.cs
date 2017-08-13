using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Office = Microsoft.Office.Core;
using Powerpoint = Microsoft.Office.Interop.PowerPoint;
using System.Windows.Forms;

namespace PowerPointAddIn1
{
	public partial class Ribbon1
	{
		private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
		{
		}
		private void buttonTitle_Click(object sender, RibbonControlEventArgs e)
		{
			Powerpoint.Presentation ppt = Globals.ThisAddIn.Application.ActivePresentation;
			var selection = Globals.ThisAddIn.Application.ActiveWindow.Selection;
			var font = selection.TextRange.Font;
			var shapeRange = selection.ShapeRange;
			float marginLeft = shapeRange.Left;
			float marginTop = shapeRange.Top;
			foreach (Powerpoint.Slide slide in ppt.Slides)
			{
				foreach (Powerpoint.Shape shape in slide.Shapes)
				{
					if (shape.HasTextFrame == Office.MsoTriState.msoTrue)
					{
						float lb = 100;
						switch (e.Control.Id)
						{
							case "buttonTitlePosition":
								if (marginTop - lb < shape.Top && shape.Top < marginTop + lb)
									if (marginLeft - lb < shape.Left && shape.Left < marginLeft + lb)
									{
										shape.Top = marginTop;
										shape.Left = marginLeft;
									}
								break;
							case "buttonTitleFont":
								if (marginTop - lb < shape.Top && shape.Top < marginTop + lb)
									if (marginLeft - lb < shape.Left && shape.Left < marginLeft + lb)
									{
										shape.TextFrame.TextRange.Font.Bold = font.Bold;
										shape.TextFrame.TextRange.Font.Color.RGB = font.Color.RGB;
										shape.TextFrame.TextRange.Font.Italic = font.Italic;
										shape.TextFrame.TextRange.Font.Shadow = font.Shadow;
										shape.TextFrame.TextRange.Font.Size = font.Size;
										shape.TextFrame.TextRange.Font.Underline = font.Underline;
									}
								break;
							default:
								break;
						}
					}
				}
			}
		}
		private void buttonFont_Click(object sender, RibbonControlEventArgs e)
		{
			Powerpoint.Presentation ppt = Globals.ThisAddIn.Application.ActivePresentation;
			Powerpoint.Selection sel = Globals.ThisAddIn.Application.ActiveWindow.Selection;
			Powerpoint.Font font = sel.TextRange.Font;
			foreach (Powerpoint.Slide page in ppt.Slides)
			{
				foreach (Powerpoint.Shape shape in page.Shapes)
				{
					if (shape.Type == Office.MsoShapeType.msoTextBox)
					{
						shape.TextFrame.TextRange.Font.Bold = font.Bold;
						shape.TextFrame.TextRange.Font.Color.RGB = font.Color.RGB;
						shape.TextFrame.TextRange.Font.Italic = font.Italic;
						shape.TextFrame.TextRange.Font.Shadow = font.Shadow;
						shape.TextFrame.TextRange.Font.Size = font.Size;
						shape.TextFrame.TextRange.Font.Underline = font.Underline;
					}
				}
			}
		}
		private void CADSelect_Click(object sender, RibbonControlEventArgs e)
		{
			if (CADSelect.Checked || true)
			{
			}
		}


	}
}
