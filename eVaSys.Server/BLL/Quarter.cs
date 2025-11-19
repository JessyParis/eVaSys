/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : eValorplast (Application de gestion d'activité)
/// Création : 16/01/2018
/// ----------------------------------------------------------------------------------------------------- 
/// </Propriété>
using System.Globalization;

/// <summary>
/// Classe pour les trimestres
/// </summary>
namespace eValorplast.BLL
{
    [Serializable]
    public class Quarter
    {
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Constructeur pour un nouvel élément
        public Quarter()
        {
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Constructeur pour un élément existant
        public Quarter(int nb, int year, CultureInfo culture)
        {
            _nb = nb;
            _begin = new DateTime(year, ((nb - 1) * 3) + 1, 1, 0, 0, 0);
            _end = new DateTime(year, (nb * 3), 1, 23, 59, 59);
            _end = _end.AddMonths(1).AddDays(-1);
            _year = year;
            Culture = culture;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Constructeur pour un élément existant à partir d'une date réelle
        public Quarter(DateTime d, CultureInfo culture)
        {
            switch (d.Month)
            {
                case 1:
                case 2:
                case 3:
                    _nb = 1;
                    break;
                case 4:
                case 5:
                case 6:
                    _nb = 2;
                    break;
                case 7:
                case 8:
                case 9:
                    _nb = 3;
                    break;
                case 10:
                case 11:
                case 12:
                    _nb = 4;
                    break;
            }
            _year = d.Year;
            _begin = new DateTime((int)_year, (((int)_nb - 1) * 3) + 1, 1, 0, 0, 0);
            _end = new DateTime((int)_year, ((int)_nb * 3), 1, 23, 59, 59);
            _end = _end.AddMonths(1).AddDays(-1);
            Culture = culture;
        }
        //-------------------------------------------------------------------
        //Numéro du trimestre
        private int? _nb = null;
        public int? Nb
        {
            get { return _nb; }
        }
        //Name
        private string _name = "";
        public string Name
        {
            get { return _name; }
        }
        //Name court
        private string _nameShort = "";
        public string NameShort
        {
            get { return _nameShort; }
        }
        //Name HTML
        private string _nameHTML = "";
        public string NameHTML
        {
            get { return _nameHTML; }
        }
        //Année
        private int? _year = null;
        public int? Year
        {
            get { return _year; }
        }
        //Date de début
        private DateTime _begin = DateTime.MinValue;
        public DateTime Begin
        {
            get { return _begin; }
        }
        //Date de end
        private DateTime _end = DateTime.MinValue;
        public DateTime End
        {
            get { return _end; }
        }
        //Culture
        private CultureInfo _culture;
        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;
                setName();
            }
        }
        //Set name
        private void setName()
        {
            switch (_nb)
            {
                case 1:
                    _name = (_culture.Name == "en-GB" ? "1st quarter" : "1er trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameShort = (_culture.Name == "en-GB" ? "1st q." : "1er trim.") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameHTML = (_culture.Name == "en-GB" ? "1<sup>st</sup>&nbsp;quarter" : "1<sup>er</sup>&nbsp;trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    break;
                case 2:
                    _name = (_culture.Name == "en-GB" ? "2nd quarter" : "2ème trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameShort = (_culture.Name == "en-GB" ? "2nd q." : "2ème trim.") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameHTML = (_culture.Name == "en-GB" ? "2<sup>er</sup>&nbsp;quarter" : "2<sup>&egrave;me</sup>&nbsp;trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    break;
                case 3:
                    _name = (_culture.Name == "en-GB" ? "3rd quarter" : "3ème trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameShort = (_culture.Name == "en-GB" ? "3rd q." : "3ème trim.") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameHTML = (_culture.Name == "en-GB" ? "3<sup>rd</sup>&nbsp;quarter" : "3<sup>&egrave;me</sup> trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    break;
                case 4:
                    _name = (_culture.Name == "en-GB" ? "4th quarter" : "4ème trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameShort = (_culture.Name == "en-GB" ? "4th q." : "4ème trim.") + (_year == 1 ? "" : " " + _year.ToString());
                    _nameHTML = (_culture.Name == "en-GB" ? "4<sup>th</sup>&nbsp;quarter" : "4<sup>&egrave;me</sup>&nbsp;trimestre") + (_year == 1 ? "" : " " + _year.ToString());
                    break;
            }
        }
    }
}
