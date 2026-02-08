using System;
using System.Collections;
using System.Collections.Generic;

using System.Drawing;

using shine.libs.math;

namespace shine.libs.drawing
{
	public class TexPad
	{
		public class TexKey
		{
			public static class KeyStyle
			{
				public const int normal=1;
				public const int space=2;
				public const int back=3;
				public const int enter=4;
			}
			public class KeyText
			{
				public string text;
				public Color color;
				public float x,y;
				public float fsize;
				
				public KeyText(string text, Color color, float fsize, float x, float y)
				{
					this.text=text;
					this.color=color;
					this.fsize=fsize;
					this.x=x;
					this.y=y;
				}
			}
			
			public List<KeyText> texts;
			public int keyStyle;
			public krect rectBmp;
			public int basKeyIndex;
			
			public TexKey()
			{
				texts=new List<KeyText>();
				keyStyle=KeyStyle.normal;
				
				rectBmp=new krect();
				basKeyIndex=0;
			}
			public void add(string text, Color color, float fsize, float x, float y)
			{
				texts.Add(new KeyText(text, color, fsize, x, y));
			}
			public void add(string text, int basKeyIndex, Color color, float fsize, float x, float y)
			{
				if(basKeyIndex>-1)
					this.basKeyIndex=basKeyIndex;
				
				add(text, color, fsize, x, y);
			}
		}
		
		public static class padID
		{
			public const int main=1;
			public const int layer=2;
			public const int path=3;
			public const int color=4;
		}
			
		
		public TexKey[] keys;
		public int count;
		
		public TexPad(int count)
		{
			this.count=count;
			
			keys=new TexKey[count];
			
			for(int i=0; i<count; i++)
				keys[i]=new TexKey();
		
		}
	}
}
