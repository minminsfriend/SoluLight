/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-05-17
 * Time: 오후 6:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using System.Drawing;
using System.Drawing.Drawing2D;

using shine.libs.math;

namespace shine.libs.drawing
{
	public static class PathType
	{
		public const int none=0;
		public const int line=1;
		public const int polygon=2;
		public const int curve=3;
		public const int closedcurve=4;
		public const int rect=5;
		public const int ellipse=6;
		public const int arc=7;
	}
	public static class FigureFlag
	{
		public const int none=0;
		public const int start=1;
		public const int end=2;
	}
	public class dvec
	{
		public kvec pos;
		public int nfigure;
		public bool selected;
		
		public dvec(kvec pos, int nfigure)
		{
			this.pos=new kvec(pos);
			this.nfigure=nfigure;
			this.selected=false;
		}
	}
		
	public class KPath
	{
		public List<dvec> poss;
		GraphicsPath path;
		
		public Color colorPen, colorBrush;
		public Brush brush;
		public Pen pen;
		public int widthPen;
		
		public int index;
		public int ntype;

		public KPath(int ntype)
		{
			this.ntype=ntype;
			
			poss=new List<dvec>();
			index=-1;
			
			colorPen=Color.Gray;
			colorBrush=Color.Bisque;
			widthPen=5;
			
			pen=new Pen(colorPen, widthPen);
			brush=new SolidBrush(colorBrush);
		}
		public void setPen(Color colorPen, int widthPen)
		{
			this.colorPen=colorPen;
			this.widthPen=widthPen;
			pen=new Pen(colorPen, widthPen);
		}
		public void setBrush(Color colorBrush)
		{
			this.colorBrush=colorBrush;
			brush=new SolidBrush(colorBrush);
		}
		
		public int Count
		{
			get{return poss.Count;}
		}
		public void add(kvec pos, int ntype)
		{
			poss.Add(new dvec(pos,ntype));
			index=poss.Count-1;
		}
		public void clear()
		{
			poss.Clear();
			index=-1;
		}
		public void removeBack()
		{
			if(poss.Count>0)
			{
				poss.RemoveAt(poss.Count-1);
				indexSet();
			}
		}
		public void remove()
		{
			int n;
			int countOld=poss.Count;
			
			for(int i=0; i<poss.Count; i++)
			{
				n=countOld-1-i;
				
				if(poss[n].selected)
					poss.RemoveAt(n);
			}
			
			indexSet();
		}
		void indexSet()
		{
			if(poss.Count==0)
				index=-1;
			else
				index=kmath.setBound(index, 0, poss.Count-1);
		}
		public void insert()
		{
			bool inserted=false;			
			dvec p0, p1;
			
			for(int i=0; i<poss.Count; i++)
			{
				p0=poss[i];
				p1=(i==poss.Count-1)? poss[0]:poss[i+1];
				
				if(p0.selected && p1.selected)
				{
					kvec pos=kvec.add(p0.pos, p1.pos);
					pos.scale(0.5f);
					
					poss.Insert(i+1, new dvec(pos,FigureFlag.none));
					index=i+1;
					
					inserted=true;
					break;
				}
			}
			
			if(inserted)
			{
				int index_1=(index+1 > poss.Count-1)? 0:index+1;
				
				poss[index-1].selected=false;
				poss[index-0].selected=true;
				poss[index_1].selected=false;
			}
		}				
		public void moveAll(kvec dis)
		{
			kvec pos;
			
			for(int i=0; i<poss.Count; i++)
			{
				pos=new kvec(poss[i].pos);
				pos.add(dis);
				
				poss[i].pos=new kvec(pos);
			}
		}
		public void move(kvec dis)
		{
			kvec pos;
			
			for(int i=0; i<poss.Count; i++)
			{
				if(poss[i].selected)
				{
					pos=poss[i].pos.copy();
					pos.add(dis);
					
					poss[i].pos=pos.copy();
				}
			}
		}
		public void select(kvec pos_, int add_flag)
		{
			kvec pos, vec;
			
			if(add_flag==0)//one selected
			{
				for(int i=0; i<poss.Count; i++)
				{
					pos=poss[i].pos.copy();
					vec=kvec.sub(pos, pos_);
					
					if(vec.length()<10)
					{
						index=i;
						poss[i].selected = true;
					}
					else
					{
						poss[i].selected = false;
					}
				}
			}
			else if(add_flag==+1 || add_flag==-1)// +1 -1 select
			{
				for(int i=0; i<poss.Count; i++)
				{
					pos=poss[i].pos.copy();
					vec=kvec.sub(pos, pos_);
					
					if(vec.length()<10)
					{
						index=i;
						poss[i].selected = add_flag==-1? false: true;
						break;
					}
				}
			}
		}
		public void selectAll()
		{
			for(int i=0; i<poss.Count; i++)
			{
				poss[i].selected=true;
			}
		}
		public void deSelectAll()
		{
			for(int i=0; i<poss.Count; i++)
			{
				poss[i].selected=false;
			}
		}
		public void togglefigure()
		{
			if(-1<index && index<poss.Count)
			{
				int nfigure=poss[index].nfigure;
				
				poss[index].nfigure=nfigure==FigureFlag.none? 
											FigureFlag.start : FigureFlag.none;
			}
		}

		public GraphicsPath getPath()
		{
			buildPath();
			return path;
		}
		
		List<int> nss,nes;
		PointF[] pss;
		
		void buildPath()
		{
			nss=new List<int>();
			nes=new List<int>();
			
			for(int i=0; i<poss.Count; i++)
			{
				if(i==0)
					nss.Add(0);
				else if(poss.Count-1==i)
					nes.Add(i);
				else if(poss[i].nfigure==FigureFlag.start)
				{
					nss.Add(i);
					nes.Add(i);
				}
			}
				
			int ns,ne, len;
			path=new GraphicsPath();
			
			for(int k=0; k<nss.Count; k++)
			{
				ns=nss[k];
				ne=nes[k];
				
				len=ne-ns+1;
				
				if(len>0 && ns+len-1<=poss.Count-1)
				{
					pss=new PointF[len];
					
					for(int i=0; i<len; i++)
						pss[i]=poss[ns+i].pos.PF;
					
					path.AddCurve(pss);
				}
			}
		}		
		void buildPath00()
		{
			path=new GraphicsPath();
			
			PointF[] points=new PointF[poss.Count];
			kvec pos;
			
			for(int i=0; i<poss.Count; i++)
			{
				pos=poss[i].pos;
				points[i]=pos.PF;
			}
			
			switch(ntype)
			{
				case PathType.line:
					path.AddLines(points);
					break;
				case PathType.polygon:
					path.AddPolygon(points);
					break;
				case PathType.curve:
					path.AddCurve(points);
					break;
				case PathType.closedcurve:
					path.AddClosedCurve(points);
					break;
				case PathType.rect:
					path.AddRectangle((new krect(points)).R);
					break;
				case PathType.arc:
					path.AddArc((new krect(points)).R,0,90);
					break;
				case PathType.ellipse:
					path.AddEllipse((new krect(points)).R);
					break;
							
			}			
		}
	}
	
	public class PathList
	{
		public List<KPath> kpaths;
		public int index;

		public PathList()
		{
			kpaths=new List<KPath>();
			index=-1;
		}
		public int Count
		{
			get{return kpaths.Count;}
		}
		public void add(KPath path)
		{
			kpaths.Add(path);
			index=kpaths.Count-1;
		}
		public void clear()
		{
			kpaths.Clear();
			index=-1;
		}
		public void remove()
		{
			if(kpaths.Count==1)
			{
			}
			else if(kpaths.Count>1)
			{
				index=kmath.setBound(index, 0, kpaths.Count-1);
				kpaths.RemoveAt(index);
				
				index=kmath.setBound(index, 0, kpaths.Count-1);
			}
		}
		public void moveIndex(int dn)
		{
			if(kpaths.Count==0)
				index=-1;
			else				
			{
				index=kmath.setBound(index+dn, 0, kpaths.Count-1);
			}
		}
		public void setIndex(int index)
		{
			if(kpaths.Count==0)
				this.index=-1;
			else				
			{
				this.index=kmath.setBound(index, 0, kpaths.Count-1);
			}
		}
		public KPath Path
		{
			get
			{
				if(-1<index && index<kpaths.Count)
					return (KPath)kpaths[index];
				else
					return null;
			}
		}
	}
}
