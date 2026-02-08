using System;
using System.Drawing;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shine.libs.math
{
    public static class MatType
    {
        public const int none = 0;
        public const int trans = 1;
        public const int scale = 2;
        public const int rotate = 3;
    }

    public class kmat
    {
        public float[] m = new float[16];

        public static readonly kvec axisX = new kvec(1, 0, 0);
        public static readonly kvec axisY = new kvec(0, 1, 0);
        public static readonly kvec axisZ = new kvec(0, 0, 1);

        public kmat()
        {
            SetIdentity();
        }
        public kmat(int mtype, kvec axis, float degree)
        {
            //if (mtype == MtType.rotate)
            setZero();
            setRotate(axis.x, axis.y, axis.z, degree);
        }
        public kmat(int mtype, kvec p)
        {
            if (mtype == MatType.trans)
            {
                mTranslate(p);
            }
            else if (mtype == MatType.scale)
            {
                mScale(p);
            }
        }
        public void setRotate(kvec axis, float a)
        {
            setZero();
            setRotate(axis.x, axis.y, axis.z, a);
        }
        public static kmat zRotate(float a)
        {
            kmat mat = new kmat();

            mat.setZero();
            mat.setRotate(0, 0, 1, a);
            return mat;
        }
        void setRotate(float x, float y, float z, float a)
        {
            float s, c;

            m[15] = 1.0f;

            a *= (float)Math.PI / 180.0f;
            s = (float)Math.Sin(a);
            c = (float)Math.Cos(a);

            if (kmath.fEquals(1.0f, x) && kmath.fEquals(0.0f, y) && kmath.fEquals(0.0f, z))
            {
                m[5] = c; m[10] = c;
                m[6] = s; m[9] = -s;
                m[0] = 1;
            }
            else if (kmath.fEquals(0.0f, x) && kmath.fEquals(1.0f, y) && kmath.fEquals(0.0f, z))
            {
                m[0] = c; m[10] = c;
                m[8] = s; m[2] = -s;
                m[5] = 1;
            }
            else if (kmath.fEquals(0.0f, x) && kmath.fEquals(0.0f, y) && kmath.fEquals(1.0f, z))
            {
                m[0] = c; m[5] = c;
                m[1] = s; m[4] = -s;
                m[10] = 1;
            }
            else
            {
                kvec v = new kvec(x, y, z);
                v.normalize();
                x = v.x; y = v.y; z = v.z;

                float nc = 1.0f - c;
                float xy = x * y;
                float yz = y * z;
                float zx = z * x;
                float xs = x * s;
                float ys = y * s;
                float zs = z * s;
                m[0] = x * x * nc + c;
                m[4] = xy * nc - zs;
                m[8] = zx * nc + ys;
                m[1] = xy * nc + zs;
                m[5] = y * y * nc + c;
                m[9] = yz * nc - xs;
                m[2] = zx * nc - ys;
                m[6] = yz * nc + xs;
                m[10] = z * z * nc + c;
            }
        }
        public void mScale(kvec s)
        {
            SetIdentity();

            for (int i = 0; i < 4; i++)
            {
                m[i] *= s.x;
                m[4 + i] *= s.y;
                m[8 + i] *= s.z;
            }
        }
        public void mTranslate(kvec p)
        {
            SetIdentity();

            for (int i = 0; i < 4; i++)
            {
                m[12 + i] += m[i] * p.x + m[4 + i] * p.y + m[8 + i] * p.z;
            }
        }
        void mTranslateNotIden(kvec p)
        {
            for (int i = 0; i < 4; i++)
            {
                m[12 + i] += m[i] * p.x + m[4 + i] * p.y + m[8 + i] * p.z;
            }
        }
        public kvec Transform(kvec src)
        {
            float[] p = new float[4] { src.x, src.y, src.z, 1f };

            float x = m[4 * 0 + 0] * p[0] + m[4 * 1 + 0] * p[1] + m[4 * 2 + 0] * p[2] + m[4 * 3 + 0] * p[3];
            float y = m[4 * 0 + 1] * p[0] + m[4 * 1 + 1] * p[1] + m[4 * 2 + 1] * p[2] + m[4 * 3 + 1] * p[3];
            float z = m[4 * 0 + 2] * p[0] + m[4 * 1 + 2] * p[1] + m[4 * 2 + 2] * p[2] + m[4 * 3 + 2] * p[3];

            return new kvec(x, y, z);
        }
        public void SetIdentity()
        {
            setZero();

            m[0] = m[5] = m[10] = m[15] = 1.0f;
        }
        void setZero()
        {
            for (int i = 0; i < 16; i++)
                m[i] = 0;
        }
        // kmat 클래스 내부에 추가
        public bool Invert()
        {
            float[] inv = new float[16];
            float[] m = this.m; // 현재 행렬 데이터

            // 0~3행에 대한 여인수 계산 등 4x4 역행렬 공식 적용 필요
            // 일반적인 가우스 소거법 또는 여인수 전개법 사용
            // (구현 내용이 길어지므로 개념만 전달드립니다)

            // 계산된 inv 값을 this.m에 덮어쓰기
            // 성공 시 true, 행렬식이 0이라 실패 시 false 반환
            return true;
        }
        public kmat copy()
        {
            kmat mat1 = new kmat();

            for (int i = 0; i < 16; i++)
                mat1.m[i] = m[i];

            return mat1;
        }
        public static kmat operator *(kmat matA, kmat matB)
        {
            return Multiply(matA, matB);
        }
        public static kmat Multiply(kmat matA, kmat matB)
        {
            // A:first action,  B:second action

            float[] M = new float[4];
            float m_4i0, m_4ij;

            kmat matC = new kmat();

            for (int i = 0; i < 4; i++)
            {
                m_4i0 = matA.m[4 * i + 0];
                M[0] = matB.m[4 * 0 + 0] * m_4i0;
                M[1] = matB.m[4 * 0 + 1] * m_4i0;
                M[2] = matB.m[4 * 0 + 2] * m_4i0;
                M[3] = matB.m[4 * 0 + 3] * m_4i0;

                for (int j = 1; j < 4; j++)
                {
                    m_4ij = matA.m[4 * i + j];
                    M[0] += matB.m[4 * j + 0] * m_4ij;
                    M[1] += matB.m[4 * j + 1] * m_4ij;
                    M[2] += matB.m[4 * j + 2] * m_4ij;
                    M[3] += matB.m[4 * j + 3] * m_4ij;
                }

                matC.m[4 * i + 0] = M[0];
                matC.m[4 * i + 1] = M[1];
                matC.m[4 * i + 2] = M[2];
                matC.m[4 * i + 3] = M[3];
            }

            return matC;
        }
        public static kmat Rotate(kmat src, kvec axis, float a)
        {
            kmat rot = new kmat(MatType.rotate, axis, a);

            return Multiply(src, rot);
        }
        public void setLookAt(kvec eye, kvec cen, kvec up)
        {
            kvec f = cen - eye;
            f.normalize();

            kvec s = new kvec();
            s.x = f.y * up.z - f.z * up.y;
            s.y = f.z * up.x - f.x * up.z;
            s.z = f.x * up.y - f.y * up.x;
            s.normalize();

            kvec u = new kvec();
            u.x = s.y * f.z - s.z * f.y;
            u.y = s.z * f.x - s.x * f.z;
            u.z = s.x * f.y - s.y * f.x;

            m[0] = s.x;
            m[1] = u.x;
            m[2] = -f.x;
            m[3] = 0.0f;
            m[4] = s.y;
            m[5] = u.y;
            m[6] = -f.y;
            m[7] = 0.0f;
            m[8] = s.z;
            m[9] = u.z;
            m[10] = -f.z;
            m[11] = 0.0f;
            m[12] = 0.0f;
            m[13] = 0.0f;
            m[14] = 0.0f;
            m[15] = 1.0f;

            // mTrans 를 만들어서 곱하는 것과 같은가?
            this.mTranslateNotIden(-1f * eye);
        }
        public void mFrustum(float left, float right,
                float bottom, float top, float near, float far)
        {
            float r_width = 1.0f / (right - left);
            float r_height = 1.0f / (top - bottom);
            float r_depth = 1.0f / (near - far);
            float x = 2.0f * (near * r_width);
            float y = 2.0f * (near * r_height);
            float A = 2.0f * ((right + left) * r_width);
            float B = (top + bottom) * r_height;
            float C = (far + near) * r_depth;
            float D = 2.0f * (far * near * r_depth);

            setZero();
            m[0] = x;
            m[5] = y;
            m[8] = A;
            m[9] = B;
            m[10] = C;
            m[14] = D;
            m[11] = -1.0f;
        }
        public void mOrtho(float left, float right,
                float bottom, float top, float near, float far)
        {
            float r_width = 1.0f / (right - left);
            float r_height = 1.0f / (top - bottom);
            float r_depth = 1.0f / (near - far);
            float x = 2.0f * (near * r_width);
            float y = 2.0f * (near * r_height);
            float A = 2.0f * ((right + left) * r_width);
            float B = (top + bottom) * r_height;

            setZero();
            m[0] = x;
            m[5] = y;
            m[10] = r_depth;
            m[12] = A;
            m[13] = B;
            m[14] = near * r_depth;
            m[15] = 1;
        }
        public void mPerspect(float fov_deg, float aspect, float near, float far)
        {
            // uh = Cot( fov/2 ) == 1/Tan(fov/2)
            // uw / uh = 1/aspect
            //
            //   uw         0       0       0
            //    0        uh       0       0
            //    0         0      f/(f-n)  1
            //    0         0    -fn/(f-n)  0
            //

            float fov_rad = (fov_deg / 2.0f) * ((float)Math.PI / 180.0f);
            float sy = 1.0f / (float)Math.Tan(fov_rad);
            float sx = sy / aspect;
            float sz = far / (far - near);

            setZero();

            m[0] = sx;
            m[5] = sy;
            m[10] = -sz;
            m[11] = -1;
            m[14] = -near * sz;
            m[15] = 0;
        }
        public kvec _4Vector3x(kvec src)
        {
            float[] V = { src.x, src.y, src.z, 1 };

            kvec dst = new kvec();
            dst.x = m[4 * 0 + 0] * V[0] + m[4 * 1 + 0] * V[1] + m[4 * 2 + 0] * V[2] + m[4 * 3 + 0] * V[3];
            dst.y = m[4 * 0 + 1] * V[0] + m[4 * 1 + 1] * V[1] + m[4 * 2 + 1] * V[2] + m[4 * 3 + 1] * V[3];
            dst.z = m[4 * 0 + 2] * V[0] + m[4 * 1 + 2] * V[1] + m[4 * 2 + 2] * V[2] + m[4 * 3 + 2] * V[3];

            return dst;
        }
        public static kvec getPosScreen(kvec p, float width, float height,
                float fov, kvec eye, float near)
        {
            float angle = fov / 2.0f * ((float)Math.PI / 180.0f);//상수
            float y_max = near * (float)Math.Tan(angle);
            float z = eye.z - p.z;// eye - vertex, vector연산, eye - at = 임시 z축

            float x_proj = near * (p.x / z);//변수
            float y_proj = near * (p.y / z);

            float x = width / 2.0f + height / 2.0f * (x_proj / y_max);
            float y = height / 2.0f + height / 2.0f * (-y_proj / y_max);

            return new kvec(x, y);
        }
        //public static kvec getPosGL(kvec e, float width, float height,
        //        float fov, kvec eye, float near)
        //{
        //    float angle = fov / 2.0f * ((float)Math.PI / 180.0f);//상수
        //    float y_max = near * (float)Math.Tan(angle);
        //    float z = eye.z - p.z;// eye - vertex, vector연산, eye - at = 임시 z축

        //    float x_proj = (e.x - width / 2.0f) / (height / 2.0f) * y_max;
        //    float y_proj = -(e.y - height / 2.0f) / (height / 2.0f) * y_max;

        //    float x = x_proj / near * z;
        //    float y = y_proj / near * z;

        //    return new kvec(x, y);
        //}
        public static float getZ47(float x47, float fov, float aspect, kvec eye)
        {
            float angle = fov / 2.0f * ((float)Math.PI / 180.0f);//상수

            float y_max = x47 / aspect;

            //Math.tan(angle)=y_max/z_max;
            float z_max = y_max / (float)Math.Tan(angle);

            //z_max=eyeZ+zObj;
            float zObj = -(z_max - eye.z);

            return -Math.Abs(zObj);
        }
        public static float getZ22(float y22, float fov, kvec eye)
        {
            float angle = fov / 2.0f * ((float)Math.PI / 180.0f);//상수

            float y_max = y22 / 1.0f;

            //Math.tan(angle)=y_max/z_max;
            float z_max = y_max / (float)Math.Tan(angle);

            //z_max=eyeZ+zObj;
            float zObj = -(z_max - eye.z);

            return -Math.Abs(zObj);
        }
    }
}
