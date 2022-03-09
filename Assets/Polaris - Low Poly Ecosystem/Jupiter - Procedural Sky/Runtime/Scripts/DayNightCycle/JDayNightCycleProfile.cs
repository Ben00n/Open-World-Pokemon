using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Pinwheel.Jupiter
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [CreateAssetMenu(menuName = "Jupiter/Day Night Cycle Profile")]
    public class JDayNightCycleProfile : ScriptableObject
    {
        private static Dictionary<string, int> propertyRemap;
        private static Dictionary<string, int> PropertyRemap
        {
            get
            {
                if (propertyRemap == null)
                {
                    propertyRemap = new Dictionary<string, int>();
                }
                return propertyRemap;
            }
            set
            {
                propertyRemap = value;
            }
        }

        static JDayNightCycleProfile()
        {
            InitPropertyRemap();
        }

        private static void InitPropertyRemap()
        {
            PropertyRemap.Clear();
            Type type = typeof(JSkyProfile);
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in props)
            {
                JAnimatableAttribute animatable = p.GetCustomAttribute(typeof(JAnimatableAttribute), false) as JAnimatableAttribute;
                if (animatable == null)
                    continue;
                string name = p.Name;
                int id = Shader.PropertyToID("_" + name);
                PropertyRemap.Add(name, id);
            }
        }

        [SerializeField]
        private List<JAnimatedProperty> animatedProperties;
        public List<JAnimatedProperty> AnimatedProperties
        {
            get
            {
                if (animatedProperties == null)
                {
                    animatedProperties = new List<JAnimatedProperty>();
                }
                return animatedProperties;
            }
            set
            {
                animatedProperties = value;
            }
        }

        public void AddProperty(JAnimatedProperty p, bool setDefaultValue = true)
        {
            if (setDefaultValue)
            {
                JDayNightCycleProfile defaultProfile = JJupiterSettings.Instance.DefaultDayNightCycleProfile;
                if (defaultProfile != null)
                {
                    JAnimatedProperty defaultProp = defaultProfile.AnimatedProperties.Find(p0 => p0.Name != null && p0.Name.Equals(p.Name));
                    if (defaultProp != null)
                    {
                        p.Curve = defaultProp.Curve;
                        p.Gradient = defaultProp.Gradient;
                    }
                }
            }

            AnimatedProperties.Add(p);
        }

        public void Animate(JSky sky, float t)
        {
            CheckDefaultProfileAndThrow(sky.Profile);

            for (int i = 0; i < AnimatedProperties.Count; ++i)
            {
                JAnimatedProperty aProp = AnimatedProperties[i];
                int id = 0;
                if (!PropertyRemap.TryGetValue(aProp.Name, out id))
                {
                    continue;
                }

                if (aProp.CurveOrGradient == JCurveOrGradient.Curve)
                {
                    sky.Profile.Material.SetFloat(id, aProp.EvaluateFloat(t));
                }
                else
                {
                    sky.Profile.Material.SetColor(id, aProp.EvaluateColor(t));
                }
            }
        }

        private void CheckDefaultProfileAndThrow(JSkyProfile p)
        {
            if (p == null)
                return;
            if (p == JJupiterSettings.Instance.DefaultProfileSunnyDay ||
                p == JJupiterSettings.Instance.DefaultProfileStarryNight)
            {
                throw new ArgumentException("Animating default sky profile is prohibited. You must create a new profile for your sky to animate it.");
            }
        }

        public bool ContainProperty(string propertyName)
        {
            return AnimatedProperties.Exists((p) => p.Name.Equals(propertyName));
        }
    }
}
