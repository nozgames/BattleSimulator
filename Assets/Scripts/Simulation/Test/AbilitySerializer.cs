using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BattleSimulator.Simulation.Assets.Scripts.Simulation.Test
{
    public class AbilityComponent
    {
    }

    public class EffectRef
    {
        [XmlAttribute]
        public string path;
    }

    public class ApplyEffect : AbilityComponent
    {
        [XmlAttribute]
        public string name;

        [XmlArray("Effects")]
        [XmlArrayItem(ElementName = "Effect")]
        public EffectRef[] effects;
    }

    public class Cooldown : AbilityComponent
    {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public float duration = 1.0f;
    }

    public class Ability
    {
        //[XmlAttribute]
        [XmlAttribute]
        public string name;

        public AbilityComponent[] components;
    }

    public class AbilitySerializer
    {
        public static void Test()
        {
            try
            {
                XmlAttributes xAttrs = new XmlAttributes();
                xAttrs.XmlArray = new XmlArrayAttribute("Components");
                xAttrs.XmlArrayItems.Add(new XmlArrayItemAttribute(typeof(ApplyEffect)));
                xAttrs.XmlArrayItems.Add(new XmlArrayItemAttribute(typeof(Cooldown)));
                xAttrs.XmlArrayItems.Add(new XmlArrayItemAttribute(typeof(AbilityComponent)));

                var overrides = new XmlAttributeOverrides();
                overrides.Add(typeof(Ability), "components", xAttrs);



#if false
                    var file = File.OpenRead(Path.Combine(Application.dataPath, "Resources", "Abilities", "CubeSwordAttack.xml"));
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Ability));
                    var ability = (serializer.ReadObject(file)) as Ability;

#else
                var fileText = File.ReadAllText(@"E:\Noz\BattleSimulator\Assets\Resources\Abilities\CubeSwordAttack.xml");
                using (StringReader reader = new StringReader(fileText))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Ability), overrides);
                    var ability = (serializer.Deserialize(reader)) as Ability;
                    Console.Write(ability);
                }
#endif
            } catch (Exception e)
            {
            }
        }
    }
}
