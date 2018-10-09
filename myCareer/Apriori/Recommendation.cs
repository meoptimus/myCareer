using myCareer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myCareer.Apriori
{
   
    public class Recommendation
    {
        myCareerEntities db = new myCareerEntities();
        public List<string> recommendedjobs(string sessionid)
        {
            if (!string.IsNullOrEmpty(sessionid))
            {
                Apriori app = new Apriori();
                Itemset item11 = new Itemset();
                itemset i = db.itemsets.Find(sessionid);
                    List<string> itemsetsList = i.items.Split(',').ToList();
                    foreach (string sp in itemsetsList)
                    {
                        item11.Add(sp);

                    }
                List<string> recommendation = new List<string>();
                List<string> finalrecommendation = new List<string>();
                List<string> jobid = new List<string>();
                var rules = app.ruleList();
                foreach (var temp in rules)
                {
                    if (temp.X.Contains(item11))
                    {
                        recommendation.Add(temp.Y.ToString());
                    }
                }
                foreach (var jobids in recommendation)
                {
                    var v = jobids.Split(',').ToList();
                    foreach (var w in v)
                    {
                        string p=w.Substring(1, w.Length - 2).ToString();
                        finalrecommendation.Add(p);
                    }
                }
                return finalrecommendation;
            }
            else
            {
                return null ;
            }
        } 
    }
}