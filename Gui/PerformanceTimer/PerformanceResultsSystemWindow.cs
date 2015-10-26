﻿/*
Copyright (c) 2015, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/
#if __ANDROID__
#else
#define USE_SYSTEM_WINDOW
#endif

using MatterHackers.Agg;
using MatterHackers.Agg.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MatterHackers.Agg.UI
{
	internal class PerformanceResultsSystemWindow : SystemWindow, IPerformanceResults
	{
		Dictionary<string, TextWidget> timers = new Dictionary<string,TextWidget>();
		FlowLayoutWidget bottomToTop;

        public static IPerformanceResults CreateResultsSystemWindow(string name)
        {
            return new PerformanceResultsSystemWindow(name);
        }

        private PerformanceResultsSystemWindow(string name)
			: base(350, 200)
		{
			Title = name;
			BackgroundColor = RGBA_Bytes.White;

			bottomToTop = new FlowLayoutWidget(FlowDirection.BottomToTop)
			{
				HAnchor = HAnchor.ParentLeftRight,
				VAnchor = VAnchor.ParentTop | VAnchor.FitToChildren,
			};

			AddChild(bottomToTop);
            UiThread.RunOnIdle(() => ShowAsSystemWindow());
		}

		public void SetTime(string name, double elapsedSeconds, int recursionCount)
		{
			if (!timers.ContainsKey(name))
			{
				TextWidget newTimeWidget = new TextWidget("waiting")
				{
					AutoExpandBoundsToText = true,
				};
				newTimeWidget.Printer.DrawFromHintedCache = true;
				timers.Add(name, newTimeWidget);
				bottomToTop.AddChild(newTimeWidget);
			}

            string outputText = "{0:0.00} ms - {1}".FormatWith(elapsedSeconds * 1000, name);
            if (recursionCount > 0)
            {
                if (recursionCount == 1)
                {
                    outputText = "|_" + outputText;
                }
                else
                {
                    outputText = new string(' ', recursionCount - 1) + "|_" + outputText;
                }
            }
            timers[name].Text = outputText;
        }
    }
}
