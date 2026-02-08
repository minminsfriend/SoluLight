/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-05-19
 * Time: 오후 11:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using shine.libs.math;
using shine.libs.bpad;

namespace shine.libs.drawing
{
	public static class condraw
	{
		public static void drawXPoints(Graphics g, List<dvec> poss)
		{
			krect rectX;
			Pen pen1=new Pen(Color.Red, 3);
			Pen pen0=new Pen(Color.Blue, 3);
			
			for(int i=0; i<poss.Count; i++)
			{
				rectX=new krect(poss[i].pos, 4, 4);
				
				if(poss[i].selected)
					g.DrawRectangle(pen1, rectX.R);
				else
					g.DrawRectangle(pen0, rectX.R);
			}
			
		}
		public static void makeImage()
		{
			krect rectImage=new krect(0,0,600,600);
			krect rect=new krect(0,0,300,100);
			krect rect0;
			
			Bitmap image = new Bitmap(rectImage.W, rectImage.H, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(image);
			
			Pen pen=new Pen(Color.White, 1);
			int r,c;
			
			for(int i=0; i<12; i++)
			{
				c=i/6;
				r=i%6;
				
				rect0=rect.copy();
				rect0.offset(300*c,100*r);
				
				if(i%2==0)
					g.FillRectangle(Brushes.Green, rect0.R);
				else
					g.FillRectangle(Brushes.Blue, rect0.R);
				
				gw.drawText(g, rect0, Color.Wheat, 24, string.Format("Path {0:D2}", i));
				g.DrawRectangle(pen, rect0.R);
			}
			
			string dir=@"c:\n작업";
			string filepath=string.Format(@"{0:s}\test out.png", dir);
			image.Save(filepath, ImageFormat.Png);			
		}		
	}
	
	public static class gw
	{
		public static void drawXPoints(Graphics g, List<dvec> poss)
		{
			krect rectX;
			Pen pen1=new Pen(Color.Red, 3);
			Pen pen0=new Pen(Color.Blue, 3);
			
			for(int i=0; i<poss.Count; i++)
			{
				rectX=new krect(poss[i].pos, 4, 4);
				
				if(poss[i].selected)
					g.DrawRectangle(pen1, rectX.R);
				else
					g.DrawRectangle(pen0, rectX.R);
			}
		}
		public static void testPaint(Graphics g)
		{
			kvec p0=new kvec(050, 050);
			kvec p1=new kvec(350, 350);
			kvec p2=new kvec(100, 550);
			
			float gap=11f;
			int count=10;
			
			testDrawLine(g, p0, p1, gap, count);
			testDrawLine(g, p1, p2, gap, count);
		}
		public static void testDrawLine(Graphics g, kvec p0, kvec p1, float gap, int count)
		{
			Color color=Color.FromArgb(100,0,0,255);
			
			Pen pen =new Pen(color, 10);
						
			g.DrawLine(pen, p0.P, p1.P);
			
			kvec vec0=kvec.sub(p1,p0);
			
			kvec axis=new kvec(0,0,1);
			kmat mrot=new kmat(MatType.rotate, axis, -90f);
			
			kvec vecX=mrot.Transform(vec0);
			vecX.normalize();
			
			kvec A, B;
			
			for(int i=0; i<count; i++)
			{
				int n=i-count/2;
				
				A=kvec.add(p0, kvec.scale(vecX, n*gap));
				B=kvec.add(p1, kvec.scale(vecX, n*gap));
				
				g.DrawLine(pen, A.P, B.P);
			}
		}
		public static void drawImageBrush(Graphics g, Bitmap image0)
		{
			kvec p0=new kvec(050, 550);
			kvec p1=new kvec(550, 050);
			kvec vec=kvec.sub(p1,p0);
			
			float deg=vec.getDeg();
			
			Bitmap image=RotateImage(image0, deg);
			
			krect rectDst0,rectDst,rectSrc;
			
			rectSrc=new krect(0,0,image.Width, image.Height);
			rectDst0=new krect(p0.x,p0.y,image.Width, image.Height);
			
			float THI=40f;
			
			int count=(int)(vec.length()/THI);

			vec.normalize();
			
			for(int i=0; i<count; i++)
			{
				kvec vec1=kvec.scale(vec, i*THI);
				rectDst=rectDst0.copy();
				rectDst.offset(vec1.x, vec1.y);
				
				g.DrawImage(image, rectDst.R, rectSrc.R, GraphicsUnit.Pixel);
			}
		}
		public static Bitmap RotateImage(Bitmap img, float rotationAngle)
		{
		    Bitmap bmp = new Bitmap(img.Width, img.Height);
		    Graphics g = Graphics.FromImage(bmp);
		
		    g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
		    g.RotateTransform(rotationAngle);
		    g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
		
		    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		    g.DrawImage(img, new Point(0, 0));
		
		    g.Dispose();
		    return bmp;
		}	
    	public static void drawPathTest(Graphics g)
        {
           GraphicsPath path = new GraphicsPath();
            path.AddArc(175, 50, 50, 50, 0, -180);
           g.DrawPath(new Pen(Color.FromArgb(255,0, 0, 255), 4), path);
 
           // Create an array of points for the curve inthe second figure.
           Point[] points = {
                        new Point(40, 60),
                        new Point(50, 70),
                        new Point(30, 90)};
 
           GraphicsPath path2 = new GraphicsPath();
 
           path2.StartFigure(); // Start the firstfigure.
           path2.AddArc(175, 50, 50, 50, 0, -180);
           path2.AddLine(100, 0, 250, 20);
           // First figure is not closed.
 
           path2.StartFigure(); // Start the secondfigure.
           path2.AddLine(50, 20, 5, 90);
           path2.AddCurve(points, 3);
           path2.AddLine(50, 150, 150, 180);
           path2.CloseFigure(); // Second figure isclosed.
 
           g.DrawPath(new Pen(Color.FromArgb(255,255, 0, 0), 2), path2);
 
        }		
		public static void drawText(Graphics g, krect rect, Color color, float fontsize, string text)
		{		
			Font font = new Font("Tahoma", fontsize, FontStyle.Bold, GraphicsUnit.Point);
			Brush brush=new SolidBrush(color);
			
			StringFormat stringFormat = new StringFormat();
			
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Near;
			
			g.DrawString(text, font, brush, rect.R, stringFormat);
		}
		public static void DrawPhrase(Graphics g, int width, int height, string phrase)
	    {
	        //g.FillRectangle(Brushes.White, 0, 0, width,height);
	
	        using (var gp = new GraphicsPath())
	        {
	            gp.AddString(phrase,
	                         FontFamily.GenericMonospace,
	                         (int)FontStyle.Bold,
	                         63f,
	                         new Point(0, 0),
	                         StringFormat.GenericTypographic);
	
	            using (GraphicsPath path = gp.Deform(width, height))
	            {
	                RectangleF bounds = path.GetBounds();
	                
	                var matrix = new Matrix();
	                var x = (width - bounds.Width) / 2 - bounds.Left;
	                var y = (height - bounds.Height) / 2 - bounds.Top;
	                matrix.Translate(x, y);
	                
	                path.Transform(matrix);
	                g.FillPath(Brushes.Black, path);
	            }
	        }
	
	        g.Flush();
	    }
	    internal static GraphicsPath Deform(this GraphicsPath path, int width, int height)
	    {
			var rnd = new Random( );
		
	        var WarpFactor = 4;
	        var xAmp = WarpFactor * width / 300d;
	        var yAmp = WarpFactor * height / 50d;
	        var xFreq = 2d * Math.PI / width;
	        var yFreq = 2d * Math.PI / height;
	        var xSeed = rnd.NextDouble() * 2 * Math.PI;
	        var ySeed = rnd.NextDouble() * 2 * Math.PI;
	        var i = 0;
	        
	        var deformed = new PointF[path.PathPoints.Length];
	        kvec p;
	        
	        foreach (PointF original in path.PathPoints)
	        {
	        	p=new kvec(original);
	        	
	            var val = xFreq * original.X + yFreq * original.Y;
	            var xOffset = (int)(xAmp * Math.Sin(val + xSeed));
	            var yOffset = (int)(yAmp * Math.Sin(val + ySeed));
	            
	            p.add(xOffset, yOffset, 0);
	            deformed[i++] = p.PF;
	        }
	
	        return new GraphicsPath(deformed,  path.PathTypes);
	    }
	}
}
