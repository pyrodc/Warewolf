/*
*  Warewolf - Once bitten, there's no going back
*  Copyright 2018 by Warewolf Ltd <alpha@warewolf.io>
*  Licensed under GNU Affero General Public License 3.0 or later. 
*  Some rights reserved.
*  Visit our website for more information <http://warewolf.io/>
*  AUTHORS <http://warewolf.io/authors.php> , CONTRIBUTORS <http://warewolf.io/contributors.php>
*  @license GNU Affero General Public License <http://www.gnu.org/licenses/agpl-3.0.html>
*/

using Dev2.Common.Interfaces.Enums.Enums;
using Dev2.Data.Interfaces;
using Dev2.Data.Interfaces.Enums;

namespace Dev2.Data.TO
{
    
    public class FormatNumberTO : IFormatNumberTO
    

    {
        #region Properties

        public string Number { get; set; }
        public string RoundingType { get; set; }
        public int RoundingDecimalPlaces { get; set; }
        public bool AdjustDecimalPlaces { get; set; }
        public int DecimalPlacesToShow { get; set; }

        #endregion Properties

        #region Constructor

        public FormatNumberTO()
        {
        }

        public FormatNumberTO(string number, enRoundingType roundingType, int roundingDecimalPlaces,
                              bool adjustDecimalPlaces, int decimalPlacesToShow)
        {
            Number = "'" + number + "'"; 
            RoundingType = Dev2EnumConverter.ConvertEnumValueToString(roundingType);
            RoundingDecimalPlaces = roundingDecimalPlaces;
            AdjustDecimalPlaces = adjustDecimalPlaces;
            DecimalPlacesToShow = decimalPlacesToShow;
        }

        public FormatNumberTO(string number, string roundingType, int roundingDecimalPlaces, bool adjustDecimalPlaces,
                              int decimalPlacesToShow)
        {
            Number = "'"+number+"'";
            RoundingType = roundingType;
            RoundingDecimalPlaces = roundingDecimalPlaces;
            AdjustDecimalPlaces = adjustDecimalPlaces;
            DecimalPlacesToShow = decimalPlacesToShow;
        }

        #endregion Constructor

        #region Methods

        public enRoundingType GetRoundingTypeEnum() => (enRoundingType)Dev2EnumConverter.GetEnumFromStringDiscription(RoundingType, typeof(enRoundingType));

        #endregion Methods
    }
}
