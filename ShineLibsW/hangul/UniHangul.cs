/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-07-01
 * Time: 오후 5:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace shine.libs.hangul
{
    class ThreeSungs
    {
        public int indexCho, indexJung, indexZong;

        public ThreeSungs()
        {
            indexCho = indexJung = indexZong = -1;
        }
        public void setIndex(int unicode)
        {
            indexCho = -1;
            indexJung = -1;
            indexZong = 0;

            if (12593 <= unicode && unicode <= 12622)
            {
                indexCho = Han.getChoIndex(unicode);
            }
            else if (Han.FIRST_HANGUL <= unicode)
            {
                int localcode = unicode - Han.FIRST_HANGUL;
                indexCho = localcode / 588;
                indexJung = (localcode % 588) / 28;
                indexZong = (localcode % 588) % 28;
            }
        }
    }
    public class UniHangul
    {
        public int uniFinis, uniCompo;
        ThreeSungs _3Sungs;
        int _compoJindo;

        public UniHangul()
        {
            uniFinis = uniCompo = -1;
            _3Sungs = new ThreeSungs();
        }
    
        public void SetCho(int nCho)
        {
            int uniCho;

            switch (_compoJindo)
            {
                case Han.Jindo.zero:
                case Han.Jindo.Cho:
                    uniCho = Han.getUnicode(nCho, -1, -1);
                    uniCompo = uniCho;
                    break;
                case Han.Jindo.Jung:
                case Han.Jindo.JungX:
                case Han.Jindo.Zong:
                case Han.Jindo.ZongX:
                    uniFinis = uniCompo;
                    uniCho = Han.getUnicode(nCho, -1, -1);
                    uniCompo = uniCho;

                    break;
            }

            if (_compoJindo == Han.Jindo.Cho)
                _compoJindo = Han.Jindo.ChoX;
            else
                _compoJindo = Han.Jindo.Cho;
        }
        public void SetJung(int nJung)
        {
            _3Sungs.setIndex(uniCompo);

            uniCompo = Han.getUnicode(_3Sungs.indexCho, nJung, 0);

            if (_compoJindo == Han.Jindo.Jung)
                _compoJindo = Han.Jindo.JungX;
            else
                _compoJindo = Han.Jindo.Jung;
        }
        public void SetZong(int nZong)
        {
            switch (_compoJindo)
            {
                case Han.Jindo.Jung:
                case Han.Jindo.JungX:
                    uniCompo += nZong;

                    break;
                case Han.Jindo.Zong:
                case Han.Jindo.ZongX:
                    _3Sungs.setIndex(uniCompo);

                    uniCompo = Han.getUnicode(_3Sungs.indexCho, _3Sungs.indexJung, nZong);

                    break;
            }

            if (_compoJindo == Han.Jindo.Zong)
                _compoJindo = Han.Jindo.ZongX;
            else
                _compoJindo = Han.Jindo.Zong;
        }
        public string FinisLetter
        {
            get
            {
                string letter = uniFinis == -1 ? null : Convert.ToChar(uniFinis).ToString();
                uniFinis = -1;
                return letter;
            }
        }
        public string CompoLetter
        {
            get
            {
                string letter = uniCompo == -1 ? null : Convert.ToChar(uniCompo).ToString();
                //uniCompo = -1;
                return letter;
            }
        }
        public void reset()
        {
            uniFinis = uniCompo = -1;
        }
        public int CompoJindo
        {
            get
            {
                if (uniCompo == -1)
                    _compoJindo = Han.Jindo.zero;

                return _compoJindo;
            }
        }
        public void Delete(ref bool deleteSurround)
        {
            int uniNew;
            int jindo = CompoJindo;
            deleteSurround = false;

            switch (jindo)
            {
                case Han.Jindo.zero:
                    deleteSurround = true;

                    break;
                case Han.Jindo.Cho:
                case Han.Jindo.ChoX:
                    uniCompo = -1;
                    _compoJindo = Han.Jindo.zero;

                    break;
                case Han.Jindo.Jung:
                case Han.Jindo.JungX:
                    _3Sungs.setIndex(uniCompo);
                    uniNew = Han.getUnicode(_3Sungs.indexCho, -1, -1);
                    uniCompo = uniNew;
                    _compoJindo = Han.Jindo.ChoX;

                    break;
                case Han.Jindo.Zong:
                case Han.Jindo.ZongX:
                    _3Sungs.setIndex(uniCompo);
                    uniNew = Han.getUnicode(_3Sungs.indexCho, _3Sungs.indexJung, 0);
                    uniCompo = uniNew;
                    _compoJindo = Han.Jindo.JungX;

                    break;
            }
        }
    }
}
