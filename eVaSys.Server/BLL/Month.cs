/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : eValorplast (Application de gestion d'activité)
/// Création : 04/12/2020
/// ----------------------------------------------------------------------------------------------------- 
/// </Propriété>
using System;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Classe for months
/// </summary>
namespace eValorplast.BLL
{
    [Serializable]
    public class Month
    {
        //------------------------------------------------------------------------------------------------------------------------------------------
        //Constructors
        public Month(int nb, int? year, CultureInfo culture)
        {
            _nb = nb;
            if (year != null) { _begin = new DateTime((int)year, (int)_nb, 1, 0, 0, 0); }
            if (year != null) { _end = (new DateTime((int)year, (int)_nb, 1, 23, 59, 59)).AddMonths(1).AddDays(-1); }
            _year = year;
            Culture = culture;
            setName();
        }
        public Month(DateTime d, CultureInfo culture)
        {
            _nb = d.Month;
            _year = d.Year;
            _begin = new DateTime((int)_year, (int)_nb, 1, 0, 0, 0); 
            _end = (new DateTime((int)_year, (int)_nb, 1, 23, 59, 59)).AddMonths(1).AddDays(-1); 
            _end = _end.AddMonths(1).AddDays(-1);
            Culture = culture;
            setName();
        }
        //-------------------------------------------------------------------
        //Utils
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
        private void setName()
        {
            if (_nb != null)
            {
                switch (_nb)
                {
                    case 1:
                        if (culture.Name == "en-GB") { _name = "January"; } else { _name = "Janvier"; }
                        break;
                    case 2:
                        if (culture.Name == "en-GB") { _name = "February"; } else { _name = "Février"; }
                        break;
                    case 3:
                        if (culture.Name == "en-GB") { _name = "March"; } else { _name = "Mars"; }
                        break;
                    case 4:
                        if (culture.Name == "en-GB") { _name = "April"; } else { _name = "Avril"; }
                        break;
                    case 5:
                        if (culture.Name == "en-GB") { _name = "May"; } else { _name = "Mai"; }
                        break;
                    case 6:
                        if (culture.Name == "en-GB") { _name = "June"; } else { _name = "Juin"; }
                        break;
                    case 7:
                        if (culture.Name == "en-GB") { _name = "July"; } else { _name = "Juillet"; }
                        break;
                    case 8:
                        if (culture.Name == "en-GB") { _name = "August"; } else { _name = "Août"; }
                        break;
                    case 9:
                        if (culture.Name == "en-GB") { _name = "September"; } else { _name = "Septembre"; }
                        break;
                    case 10:
                        if (culture.Name == "en-GB") { _name = "October"; } else { _name = "Octobre"; }
                        break;
                    case 11:
                        if (culture.Name == "en-GB") { _name = "November"; } else { _name = "Novembre"; }
                        break;
                    case 12:
                        if (culture.Name == "en-GB") { _name = "December"; } else { _name = "décembre"; }
                        break;
                }
            }
            else
            {
                _name = "";
            }
        }
        //-------------------------------------------------------------------
        //Properties
        //Culture
        public CultureInfo culture { get; set; } = new CultureInfo("fr-FR");
        //Month number
        private int? _nb = null;
        public int? Nb
        {
            get { return _nb; }
        }
        //Name
        private string _name = "";
        public string Name
        {
            get { 
                return _name; }
        }
        //Year
        private int? _year = null;
        public int? Year
        {
            get { return _year; }
        }
        //Begin date
        private DateTime _begin = DateTime.MinValue;
        public DateTime Begin
        {
            get { return _begin; }
        }
        //End date
        private DateTime _end = DateTime.MinValue;
        public DateTime End
        {
            get { return _end; }
        }
    }
}
