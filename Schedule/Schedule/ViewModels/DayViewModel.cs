using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Schedule.Models;
using System.Linq;
using Newtonsoft.Json;

namespace Schedule.ViewModels
{
    class DayViewModel
    {
        public List<Couple> Couples { get; set; }
        public Dictionary<byte, TeacherCouple> TeacherCouples { get; set; }
        public List<TeacherCouple> TeacherCoupleList { get; set; }

        List<Faculty> FacultiesMain { get; set; }

        public DayViewModel(string dayOfWeek)
        {
            #region Определяется номер недели (numOfWeek)
            string numOfWeek;
            if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
            {
                numOfWeek = (string)num;
            }
            else
            {
                numOfWeek = "1";
            }
            #endregion

            //проверяется студент или преподаватель
            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
            {
                if ((bool)isTeacher)
                {
                    #region Получение расписания для преподавателя
                    if (App.Current.Properties.TryGetValue("teacherName", out object AppTeacherName))
                    {
                        string currentTeacher = (string)AppTeacherName;
                        FacultiesMain = App.facultiesMain;

                        GetDaysForTeacher(numOfWeek, dayOfWeek, currentTeacher);
                    }
                    #endregion

                }
                else 
                {
                    #region Получение расписания для студента
                    if (App.Current.Properties.TryGetValue("facultyName", out object FacultyName))
                    {
                        string facultyName = (string)FacultyName;
                        if (App.Current.Properties.TryGetValue("groupId", out object GroupId))
                        { 
                            string groupId = (string)GroupId;
                            if (App.Current.Properties.TryGetValue("groupName", out object GroupName))
                            { 
                                string groupName = (string)GroupName;
                                string subgroup = "";
                                if (App.Current.Properties.TryGetValue("subgroup", out object Subgroup))
                                {
                                    subgroup = (string)Subgroup;
                                }

                                FacultiesMain = App.facultiesMain;

                                GetDaysForStudent(numOfWeek, dayOfWeek, facultyName, groupId, groupName, subgroup);
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        public void GetDaysForTeacher(string numOfWeek, string dayOfWeek, string thisTeacher)
        {
            TeacherCouples = new Dictionary<byte, TeacherCouple>();

            foreach (var f in FacultiesMain)
            {
                foreach (var g in f.Groups)
                {
                    foreach (var c in g.Couples)
                    {
                        //Contains, так как в паре английского может быть несколько преподавателей
                        if (c.CoupleTeacher.Contains(thisTeacher))
                        {
                            if (c.Week == numOfWeek)
                            {
                                if (c.Day == dayOfWeek)
                                {
                                    byte coupleNum = Convert.ToByte(c.CoupleNum);
                                    //проверка на наличие пары с таким номером
                                    if (!TeacherCouples.ContainsKey(coupleNum))
                                    {
                                        //Если пары с таким номером еще нет, то пара просто добавляется в коллекцию
                                        TeacherCouples.Add(coupleNum, new TeacherCouple(c, g.GroupId + (c.SubgroupId != null ? "(" + c.SubgroupId + ")" : "")));
                                    }
                                    else
                                    {
                                        //Если пара с таким номером уже есть, то в поле CoupleTeacher этой пары дописывается номер новой группы
                                        TeacherCouples[coupleNum].CoupleTeacher += ", " + g.GroupId + (c.SubgroupId != null ? "(" + c.SubgroupId + ")" : "");
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

        public void GetDaysForStudent(string numOfWeek, string dayOfWeek, 
                    string facultyName, string groupId, string groupName, string subgroup)
        {
            Couples = new List<Couple>();

            foreach (var f in App.facultiesMain)
            {
                if (f.FacultyName == facultyName)
                {
                    foreach (var g in f.Groups)
                    {
                        if (g.GroupId == groupId && g.GroupName == groupName)
                        {
                            foreach (var c in g.Couples)
                            {
                                if (c.Week == numOfWeek && (c.SubgroupName == subgroup || c.SubgroupName == null) && c.Day == dayOfWeek)
                                {
                                    Couples.Add(c);
                                }
                            }
                        }
                    }
                }
            }
           
        }

    }
}
