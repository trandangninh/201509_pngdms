using Entities.Domain.ClassificationDefects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service.ClassificationDefects
{
    public class MaterialService : IMaterialService
    {
        /// <summary>
        /// get all materials
        /// </summary>
        /// <returns>list material</returns>
        public IList<Material> GetAllMaterials()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlDataExtendsion/Materials.xml";
            var listMaterials = new List<Material>();
            XElement root = XElement.Load(path);
            IEnumerable<XElement> rootNode = from el in root.Elements("Material")
                                            select el;

            foreach (XElement elm in rootNode)
            {
                var item = new Material()
                {
                    Id = int.Parse(elm.Element("Id").Value),
                    Name = elm.Element("Name").Value,
                };
                listMaterials.Add(item);

            }
            return listMaterials;
        }

        /// <summary>
        /// get material by material identity
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns>Material</returns>
        public Material GetMaterialById(int materialId)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "XmlDataExtendsion/Materials.xml";;
            XElement root = XElement.Load(path);
            IEnumerable<XElement> rootNode = from el in root.Elements("Material")
                                             select el;

            foreach (XElement elm in rootNode)
            {
                int id = int.Parse(elm.Element("Id").Value);

                if (id == materialId)
                    return new Material()
                        {
                            Id = id,
                            Name = elm.Element("Name").Value,
                        };
            }

            return null;
        }
    }
}
