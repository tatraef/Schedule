using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Schedule.Models;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Schedule.ViewModels
{
    class DesireViewModel
    {
        public int[] Monday { get; set; }
        public int[] Tuesday { get; set; }
        public int[] Wednesday { get; set; }
        public int[] Thursday { get; set; }
        public int[] Friday { get; set; }
        public int[] Saturday { get; set; }

        public string Message { get; set; }

        public DesireViewModel()
        {

            if (App.Current.Properties.TryGetValue("desire", out object desire))
            {
                Desire des = JsonConvert.DeserializeObject<Desire>(desire.ToString());
                Monday = StringToIntArray(des.Понедельник);
                Tuesday = StringToIntArray(des.Вторник);
                Wednesday = StringToIntArray(des.Среда);
                Thursday = StringToIntArray(des.Четверг);
                Friday = StringToIntArray(des.Пятница);
                Saturday = StringToIntArray(des.Суббота);
                Message = des.Message;
            }
            else
            {
                Monday = new int[] { 1, 1, 1, 1, 1 };
                Tuesday = new int[] { 1, 1, 1, 1, 1 };
                Wednesday = new int[] { 1, 1, 1, 1, 1 };
                Thursday = new int[] { 1, 1, 1, 1, 1 };
                Friday = new int[] { 1, 1, 1, 1, 1 };
                Saturday = new int[] { 1, 1, 1, 1, 1 };

                Message = "";
            }
            
        }

        private int[] StringToIntArray(string desire)
        {
            char[] desArr = desire.ToCharArray();
            int[] some = new int[desire.Length];
            for (int i = 0; i < desArr.Length; i++)
            {
                some[i] = Convert.ToInt32(desArr[i].ToString());
            }

            return some;
        }
    }
}
