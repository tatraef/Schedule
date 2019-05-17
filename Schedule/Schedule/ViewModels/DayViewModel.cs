using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Schedule.Models;
using System.Linq;

namespace Schedule.ViewModels
{
    class DayViewModel
    {
        public List<Couple> Couples { get; set; }
        public Dictionary<byte, TeacherCouple> TeacherCouples { get; set; }
        public List<TeacherCouple> TeacherCoupleList { get; set; }

        public DayViewModel(string dayOfWeek)
        {
             //проверяется студент или преподаватель
            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
            {
                /************************Преподаватель***************************************/
                if ((bool)isTeacher)
                {
                    TeacherCouples = new Dictionary<byte, TeacherCouple>();
                    //проверяется имя преподавателя
                    if (App.Current.Properties.TryGetValue("teacherName", out object AppTeacherName))
                    {
                        string thisTeacher = (string) AppTeacherName;
                        foreach (var f in App.facultiesJSON)
                        {
                            foreach (var g in f.Groups)
                            {
                                foreach (var c in g.Couples)
                                {
                                    //Contains, так как в паре английского может быть несколь преподавателей
                                    if (c.CoupleTeacher.Contains(thisTeacher)) 
                                    {
                                        if (c.Day == dayOfWeek)
                                        {
                                            try
                                            {


                                                byte coupleNum = Convert.ToByte(c.CoupleNum);
                                                if (!TeacherCouples.ContainsKey(coupleNum))
                                                {
                                                    TeacherCouples.Add(coupleNum, new TeacherCouple(c, g.GroupId));
                                                }
                                                else
                                                {
                                                    TeacherCouples[coupleNum].CoupleGroups += ", " + g.GroupId;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                string s = e.Message;
                                                throw;
                                            }
                                        }      
                                    }
                                }
                            }
                        }
                    }
                    //сортировка пар, так как могут находится не в правильном порядке
                    if (TeacherCouples != null)
                    {
                        SortedDictionary<byte, TeacherCouple> sortedTeacherCouples = new SortedDictionary<byte, TeacherCouple>(TeacherCouples);
                        TeacherCoupleList = sortedTeacherCouples.Values.ToList();
                    }
                }
            }
        }
    }
}
