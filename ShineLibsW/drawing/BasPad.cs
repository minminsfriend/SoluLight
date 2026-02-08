/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-07-16
 * Time: 오후 1:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

using shine.libs.math;

namespace shine.libs.drawing
{
	/// <summary>
	/// Description of BasPad.
	/// </summary>
	public class BasPad
	{
		public class BasKey
		{
			public krect rect;
			
			public BasKey()
			{
				rect=new krect();
			}
			
			public void setRect(float x, float y, float w, float h)
			{
				rect.setxywh(x,y,w,h);
			}
		}
		
		public BasKey[] keys;
		public Bitmap bmpBase;
		
		public BasPad(int count)
		{
			keys=new BasKey[count];
			
			for(int i=0; i<count; i++)
				keys[i]=new BasKey();
		}
	}
}
