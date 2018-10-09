using myCareer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace myCareer.Apriori
{
    public class Apriori
    {
        myCareerEntities db = new myCareerEntities();
        public string str;
        List<AssociationRule> rules;

        public Apriori()
        {
            ItemsetCollection data = new ItemsetCollection();

            List<itemset> temp = db.itemsets.ToList();
            foreach (var item in temp)
            {
                Itemset item11 = new Itemset();
                List<string> itemsetsList = item.items.Split(',').ToList();
                foreach (string sp in itemsetsList)
                {
                    item11.Add(sp);

                }
                data.Add(item11);
            }
            StringBuilder s = new StringBuilder();
            Itemset uniqueItems = data.GetUniqueItems();
            s.Append(uniqueItems + "<br/>");
            //test apriori
            ItemsetCollection L = AprioriMining.DoApriori(data, 30.0);
            s.Append("<br/>" + L.Count + " itemsets in L<br/>");
            foreach (Itemset i in L)
            {
                s.Append(i + "<br/>");
            }

            //test mining
            List<AssociationRule> allRules = AprioriMining.Mine(data, L, 70.0);
            rules = allRules;
            s.Append("<br/>" + allRules.Count + " rules<br/>");
            foreach (AssociationRule rule in allRules)
            {
                s.Append(rule + "<br/>");
            }
            str = s.ToString();

        }
        public string show()
        {
            return str;
        }
        public List<AssociationRule> ruleList()
        {
            return rules;
        }
    }
}