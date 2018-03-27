using Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Pricing.Models.TankCalculation
{
    public class ItemList { public Object item; public int count; }

    public class PrismaticTank
    {        
        Double width = 0.00;
        Double length = 0.00;
        Double height = 0.00;

        Double moduleSize = 1.08;

        public Double capacity(){ return width * length * height; }

        List<ItemList> moduleList = new List<ItemList>();
        List<ItemList> topModuleList = new List<ItemList>();
        List<ItemList> bottomModuleList = new List<ItemList>();

        int numberOfVerticalModules = 0;
        int numberOfWidthModules = 0;
        int numberOfLengthModules = 0;


        public List<Price> calculatePrice(
            Double width, Double length, Double height, 
            String topModuleMaterialThickness, String topModuleMaterial,
            String bottomModuleMaterialThickness, String bottomModuleMaterial)
        {
            this.width = width;
            this.length = length;
            this.height = height;

            Material material = new Material();
            
            getSideModuleBreakdown("width");
            getSideModuleBreakdown("length");
            this.topModuleList = getTopBottomModuleBreakdown(true, material.setPlateMaterial(topModuleMaterialThickness, topModuleMaterial));
            this.bottomModuleList = getTopBottomModuleBreakdown(false, material.setPlateMaterial(bottomModuleMaterialThickness, bottomModuleMaterial));

            //calculateTopModulePrice(topModuleList);

            return calculateTopOrBottomModulePrice(false, bottomModuleList);
        }

        private List<ItemList> getTopBottomModuleBreakdown(Boolean isTopModule, Material.Plate material)
        {
            List<ItemList> temp = new List<ItemList>();
            List<ItemList> temp1 = new List<ItemList>();

            Boolean continueOptimisation = true;

            TopOrBottomModule module = new TopOrBottomModule();

            if (isTopModule) { module.material = material; }
            else { module.material = material; }

            while (continueOptimisation)
            {
                Double lengthSize = moduleSize * 2;
                Double widthSize = moduleSize;

                if (temp.Count > 0)
                {
                    continueOptimisation = false;
                    temp1 = temp;
                    temp = new List<ItemList>();

                    lengthSize = moduleSize;
                    widthSize = moduleSize * 2;
                }

                int numberOfLength = (int)Math.Ceiling(length / lengthSize);
                int numberOfWidth = (int)Math.Ceiling(width / widthSize);

                if (numberOfWidth == 1 && numberOfLength == 1)
                {
                    if (isTopModule) { }
                    module = new TopOrBottomModule();
                    module.material = material;
                    module.length = length;
                    module.width = width;

                    ItemList materials = new ItemList();
                    materials.count = 1;
                    materials.item = module;
                    temp.Add(materials);
                }
                else if (numberOfWidth == 1 && numberOfLength > 1)
                {
                    module = new TopOrBottomModule();
                    module.material = material;
                    module.length = lengthSize;
                    module.width = width;

                    ItemList materials = new ItemList();
                    materials.count = (int)Math.Floor(length / lengthSize);
                    materials.item = module;
                    temp.Add(materials);

                    if (length % lengthSize > 0)
                    {
                        module = new TopOrBottomModule();
                        module.material = material;
                        module.length = Math.Round(length % lengthSize, 2);
                        module.width = width;

                        materials = new ItemList();
                        materials.count = 1;
                        materials.item = module;
                        temp.Add(materials);
                    }
                }
                else if (numberOfWidth > 1 && numberOfLength == 1)
                {
                    module = new TopOrBottomModule();
                    module.material = material;
                    module.length = length;
                    module.width = widthSize;

                    ItemList materials = new ItemList();
                    materials.count = (int)Math.Floor(width / widthSize);
                    materials.item = module;
                    temp.Add(materials);

                    if (width % widthSize > 0)
                    {
                        module = new TopOrBottomModule();
                        module.material = material;
                        module.length = length;
                        module.width = Math.Round(width % widthSize, 2);

                        materials = new ItemList();
                        materials.count = 1;
                        materials.item = module;
                        temp.Add(materials);
                    }
                }
                else if (numberOfWidth > 1 && numberOfLength > 1)
                {
                    module = new TopOrBottomModule();
                    module.material = material;
                    module.length = lengthSize;
                    module.width = widthSize;

                    ItemList materials = new ItemList();
                    materials.count = (int)Math.Floor(width / widthSize) * (int)Math.Floor(length / lengthSize);
                    materials.item = module;
                    temp.Add(materials);

                    if (width % widthSize > 0)
                    {
                        module = new TopOrBottomModule();
                        module.material = material;
                        module.length = lengthSize;
                        module.width = Math.Round(width % widthSize, 2);

                        materials = new ItemList();
                        materials.count = (int)Math.Floor(length / lengthSize);
                        materials.item = module;
                        temp.Add(materials);
                    }
                    if (length % lengthSize > 0)
                    {
                        module = new TopOrBottomModule();
                        module.material = material;
                        module.length = Math.Round(length % lengthSize, 2);
                        module.width = widthSize;

                        materials = new ItemList();
                        materials.count = (int)Math.Floor(width / widthSize);
                        materials.item = module;
                        temp.Add(materials);

                        if (width % widthSize > 0)
                        {
                            module = new TopOrBottomModule();
                            module.material = material;
                            module.length = Math.Round(length % lengthSize, 2);
                            module.width = Math.Round(width % widthSize, 2);

                            materials = new ItemList();
                            materials.count = 1;
                            materials.item = module;
                            temp.Add(materials);
                        }
                    }
                }
            }

            if (temp1.Count > temp.Count) { return temp; }
            else { return temp1; }
        }

        private void getSideModuleBreakdown(String dimensionType)
        {
            Double dimensionSize = 0.00;

            if (dimensionType.Equals("width")) { dimensionSize = width; numberOfWidthModules = (int)Math.Ceiling(width / moduleSize); }
            else if (dimensionType.Equals("length")) { dimensionSize = length; numberOfLengthModules = (int)Math.Ceiling(length / moduleSize); }

            numberOfVerticalModules = (int)Math.Ceiling(height / moduleSize);

            Module module = new Module();

            if (dimensionSize <= moduleSize)
            {
                if (numberOfVerticalModules == 1)
                {
                    module = new Module();
                    module.height = this.height;
                    module.width = dimensionSize;

                    ItemList materials = new ItemList();
                    materials.count = 2;
                    materials.item = module;
                    moduleList.Add(materials);
                }
                else
                {
                    module = new Module();
                    module.height = moduleSize;
                    module.width = dimensionSize;

                    ItemList materials = new ItemList();
                    materials.count = 2 * (int)Math.Floor(height / moduleSize);
                    materials.item = module;
                    moduleList.Add(materials);

                    if (height % moduleSize > 0)
                    {
                        module = new Module();
                        module.height = Math.Round(height % moduleSize, 2);
                        module.width = dimensionSize;

                        materials = new ItemList();
                        materials.count = 2;
                        materials.item = module;
                        moduleList.Add(materials);
                    }
                }

            }
            else
            {
                if (numberOfVerticalModules == 1)
                {
                    module = new Module();
                    module.height = height;
                    module.width = moduleSize;

                    ItemList materials = new ItemList();
                    materials.count = 2 * (int)Math.Floor(dimensionSize / moduleSize);
                    materials.item = module;
                    moduleList.Add(materials);

                    if (dimensionSize % moduleSize > 0)
                    {
                        module = new Module();
                        module.height = height;
                        module.width = Math.Round(dimensionSize % moduleSize, 2);

                        materials = new ItemList();
                        materials.count = 2;
                        materials.item = module;
                        moduleList.Add(materials);
                    }
                }
                else
                {
                    module = new Module();
                    module.height = moduleSize;
                    module.width = moduleSize;

                    ItemList materials = new ItemList();
                    materials.count = 2 * (int)Math.Floor(dimensionSize / moduleSize) * (int)Math.Floor(height / moduleSize);
                    materials.item = module;
                    moduleList.Add(materials);

                    if (height % moduleSize == 0)
                    {
                        module = new Module();
                        module.height = moduleSize;
                        module.width = Math.Round(dimensionSize % moduleSize, 2);

                        materials = new ItemList();
                        materials.count = 2;
                        materials.item = module;
                        moduleList.Add(materials);
                    }
                    else if (height % moduleSize > 0)
                    {
                        module = new Module();
                        module.height = Math.Round(height % moduleSize,2);
                        module.width = moduleSize;

                        materials = new ItemList();
                        materials.count = 2;
                        materials.item = module;
                        moduleList.Add(materials);

                        module = new Module();
                        module.height = Math.Round(height % moduleSize, 2);
                        module.width = Math.Round(dimensionSize % moduleSize, 2);

                        materials = new ItemList();
                        materials.count = 2;
                        materials.item = module;
                        moduleList.Add(materials);

                        if (dimensionSize % moduleSize > 0)
                        {
                            module = new Module();
                            module.height = moduleSize;
                            module.width = Math.Round(dimensionSize % moduleSize, 2);

                            materials = new ItemList();
                            materials.count = 2;
                            materials.item = module;
                            moduleList.Add(materials);
                        }
                    }
                }
            }
        }

        List<Price> calculateTopOrBottomModulePrice(Boolean isTopModule, List<ItemList> itemList)
        {
            List<Price> result = new List<Price>();
            
            List<SqlParameter> parameterList = new List<SqlParameter>();
            SqlParameter parameter = new SqlParameter();

            foreach (ItemList item in itemList)
            {
                Price price = new Price();

                TopOrBottomModule module = (TopOrBottomModule)item.item;

                parameterList = new List<SqlParameter>();
                parameter = new SqlParameter();

                parameter.ParameterName = "@materialId";
                parameter.Value = module.material.materialId; 
                parameterList.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@thicknessId";
                parameter.Value = module.material.thickness; 
                parameterList.Add(parameter);

                DataRow row = DataOperation.select("serviceManager.GetMaterialPrice", parameterList).Rows[0];

                price.value = (Decimal.Parse(row["price"].ToString())) * item.count;
                price.currency = row["currency"].ToString();
                price.description = "rawMaterial";

                result.Add(price);

                if (!price.currency.Equals(row["currency"].ToString()) && !price.currency.Equals("")) { return new List<Price>(); }
                else { price.currency = row["currency"].ToString(); }

                //ProductionCost
                parameterList = new List<SqlParameter>();
                parameter = new SqlParameter();

                parameter.ParameterName = "@itemName";
                if (isTopModule) { parameter.Value = "topPlate"; }
                else { parameter.Value = "bottomPlate"; }
                parameterList.Add(parameter);

                foreach (DataRow newRow in DataOperation.select("serviceManager.GetMaterialProductionCost", parameterList).Rows)
                {
                    if (Decimal.Parse(newRow["cost"].ToString()) <= 0) { continue; }
                    price = new Price();
                    price.description = newRow["productionProcess"].ToString();
                    price.value += (Decimal.Parse(newRow["cost"].ToString())) * item.count;
                    price.currency = newRow["currency"].ToString();
                    
                    result.Add(price);

                    if (!price.currency.Equals(newRow["currency"].ToString()) && !price.currency.Equals("")) { return new List<Price>(); }
                    else { price.currency = newRow["currency"].ToString(); }
                }

            }

            return result;
        }
    }
}