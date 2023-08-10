#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

/*
//=====================================================================================================
//   OpenSource code distributed under GPL3.0 or later (https://opensource.org/licenses/GPL-3.0)
//=====================================================================================================
//  This open source indicator was developed to aid users in knowing when bars are going to close.
//  Although this is just a supportive tool and no strategies can be built upon it, I am not resposible for any losses incurred during its use.
//  IF REDISTRIBUTING, YOU MUST INCLUDE THE SOURCE CODE AS PER GPL3.0 GUIDELINES.
//=====================================================================================================


  Author: arias-code (aria.lopez.dev@gmail.com)
  Current Version: 0.0.1
  Known issues:

   ~ Happy Trading :) 
*/

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.Sakura
{
	public class SakuraFloatingBarTimer : Indicator
	{
        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Floating bar timer for Tick/Minute/Second charts.";
				Name										= "SakuraFloatingBarTimer";
				Calculate									= Calculate.OnEachTick;
				DisplayInDataBox	= false;
				DrawOnPricePanel	= false;
				IsChartOnly			= true;
				IsOverlay			= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				IsSuspendedWhileInactive					= true;
				// Variables
				TextFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 12);
				MarginToRight = 15;
				HeightMargin = 0;
			}
			else if (State == State.Configure)
			{
			}
		}

		private string periodType;
		private int barMax;
		private Dictionary<string, bool> supportedTimes = new Dictionary<string, bool>()
		{
			{ "Tick", true },
			{ "Minute", true },
			{ "Second", true },
		};
		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
			{
				string[] chartString = Bars.ToChartString().Split('(');
				string cleanedData = chartString[chartString.Length - 1];
				cleanedData = cleanedData.Remove(cleanedData.Length - 1, 1);
				string[] parsedData = cleanedData.Split(' ');
				if (supportedTimes.ContainsKey(parsedData[1].ToString()))
				{
					periodType = parsedData[1].ToString();
					barMax = Int32.Parse(parsedData[0].ToString());
				}
			}
		}

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
			//SETUP
			base.OnRender(chartControl, chartScale);
			if (Bars == null || ChartControl == null)
				return;

			// POSITION LOGIC
			float x = (chartControl.GetXByBarIndex(ChartBars, CurrentBar) + MarginToRight);
			float y = chartScale.GetYByValue((High.GetValueAt(CurrentBar) - ((High.GetValueAt(CurrentBar) - Low.GetValueAt(CurrentBar)) / 2)) + HeightMargin);

			// Setting text to draw
			string text = getDistance(Bars.PercentComplete, Bars.TickCount);

            //DRAW
            DrawTextLayout(
				RenderTarget,
				ChartPanel,
				x,
				y,
				text,
				TextFont,
				TextColor
			);
        }

        private void DrawTextLayout(SharpDX.Direct2D1.RenderTarget renderTarget, ChartPanel chartPanel, float x, float y, string text, SimpleFont font, Brush b)
		{
			SharpDX.Vector2 origin = new SharpDX.Vector2(x, y);
			SharpDX.DirectWrite.TextFormat textFormat = font.ToDirectWriteTextFormat();
			SharpDX.DirectWrite.TextLayout textLayout = new SharpDX.DirectWrite.TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory,
														text, textFormat, chartPanel.X + chartPanel.W,
														textFormat.FontSize);
			SharpDX.Direct2D1.Brush dxBrush = b.ToDxBrush(renderTarget);
			
			renderTarget.DrawTextLayout(origin, textLayout, dxBrush);
			textFormat.Dispose();
			textFormat = null;
			textLayout.Dispose();
			textLayout = null;
		}

		private string getDistance(double percent, int tickCount = 0)
		{
			if (periodType == "Tick")
				return (Math.Abs(barMax - tickCount)).ToString();
			else if (periodType == "Second")
				return (Math.Abs(barMax - Math.Floor(barMax * percent))).ToString();
			else if (periodType == "Minute")
			{
				int maxSeconds = barMax * 60;
				int secondsPassed = (int)Math.Floor(maxSeconds * percent);
				int minutesLeft = (maxSeconds - secondsPassed) / 60;
				int secondsLeft = (maxSeconds - secondsPassed) % 60;
				return minutesLeft.ToString() + "m " + secondsLeft.ToString() + "s";
			}
			else return "";
		}

		#region Properties
		[NinjaScriptProperty]
        	[Display(Name = "Font, size, type, style", Description = "Select font, style, size to display on chart", GroupName = "Text", Order = 1)]
        	public Gui.Tools.SimpleFont TextFont
        	{ get; set; }

        	[NinjaScriptProperty]
        	[Display(Name = "Text Color", Description = "Color of text", Order = 2, GroupName = "Text")]
        	public Brush TextColor
        	{ get; set; }
        	[Browsable(false)]
        	public string TextColorSerialize
        	{
            	get { return Serialize.BrushToString(TextColor); }
            	set { TextColor = Serialize.StringToBrush(value); }
        	}

        	[NinjaScriptProperty]
		[Display(Name = "MarginToRight", Description = "Distance from bar to timer", Order = 3, GroupName = "Display Options")]
		public int MarginToRight
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name = "HeightMargin", Description = "How far above/below the bar the timer is", Order = 4, GroupName = "Display Options")]
		public float HeightMargin 
		{ get; set; }
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private Sakura.SakuraFloatingBarTimer[] cacheSakuraFloatingBarTimer;
		public Sakura.SakuraFloatingBarTimer SakuraFloatingBarTimer(Gui.Tools.SimpleFont textFont, Brush textColor, int marginToRight, float heightMargin)
		{
			return SakuraFloatingBarTimer(Input, textFont, textColor, marginToRight, heightMargin);
		}

		public Sakura.SakuraFloatingBarTimer SakuraFloatingBarTimer(ISeries<double> input, Gui.Tools.SimpleFont textFont, Brush textColor, int marginToRight, float heightMargin)
		{
			if (cacheSakuraFloatingBarTimer != null)
				for (int idx = 0; idx < cacheSakuraFloatingBarTimer.Length; idx++)
					if (cacheSakuraFloatingBarTimer[idx] != null && cacheSakuraFloatingBarTimer[idx].TextFont == textFont && cacheSakuraFloatingBarTimer[idx].TextColor == textColor && cacheSakuraFloatingBarTimer[idx].MarginToRight == marginToRight && cacheSakuraFloatingBarTimer[idx].HeightMargin == heightMargin && cacheSakuraFloatingBarTimer[idx].EqualsInput(input))
						return cacheSakuraFloatingBarTimer[idx];
			return CacheIndicator<Sakura.SakuraFloatingBarTimer>(new Sakura.SakuraFloatingBarTimer(){ TextFont = textFont, TextColor = textColor, MarginToRight = marginToRight, HeightMargin = heightMargin }, input, ref cacheSakuraFloatingBarTimer);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Sakura.SakuraFloatingBarTimer SakuraFloatingBarTimer(Gui.Tools.SimpleFont textFont, Brush textColor, int marginToRight, float heightMargin)
		{
			return indicator.SakuraFloatingBarTimer(Input, textFont, textColor, marginToRight, heightMargin);
		}

		public Indicators.Sakura.SakuraFloatingBarTimer SakuraFloatingBarTimer(ISeries<double> input , Gui.Tools.SimpleFont textFont, Brush textColor, int marginToRight, float heightMargin)
		{
			return indicator.SakuraFloatingBarTimer(input, textFont, textColor, marginToRight, heightMargin);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Sakura.SakuraFloatingBarTimer SakuraFloatingBarTimer(Gui.Tools.SimpleFont textFont, Brush textColor, int marginToRight, float heightMargin)
		{
			return indicator.SakuraFloatingBarTimer(Input, textFont, textColor, marginToRight, heightMargin);
		}

		public Indicators.Sakura.SakuraFloatingBarTimer SakuraFloatingBarTimer(ISeries<double> input , Gui.Tools.SimpleFont textFont, Brush textColor, int marginToRight, float heightMargin)
		{
			return indicator.SakuraFloatingBarTimer(input, textFont, textColor, marginToRight, heightMargin);
		}
	}
}

#endregion
