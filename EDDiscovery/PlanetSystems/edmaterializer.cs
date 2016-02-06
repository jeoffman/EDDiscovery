﻿using EDDiscovery2.HTTP;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace EDDiscovery2.PlanetSystems
{
    public class EdMaterializer : EDMaterizliaerCom
    {


        public EdMaterializer()
        {
#if DEBUG
            // Dev server. Mess with data as much as you like here
            _serverAddress = "https://ed-materializer.herokuapp.com/";
            //_serverAddress = "http://ed-materializer-env.elasticbeanstalk.com/";
#else
            // Production
            _serverAddress = "http://ed-materializer-env.elasticbeanstalk.com/";
#endif
        }


        public List<EDPlanet>GetAllPlanets(string system)
        {
            List<EDPlanet> listObjects = new List<EDPlanet>();
            string query = "api/v1/world_surveys";

            if (!String.IsNullOrEmpty(system))
                query = query + "/?q[system]="+HttpUtility.UrlEncode(system);

            var response = RequestGet(query);
            var json = response.Body;

            JArray jArray = null;
            JObject jObject = null;
            if (json != null && json.Length > 5)
                jObject = (JObject)JObject.Parse(json);

            if (jObject == null)
                return listObjects;


            jArray = (JArray)jObject["world_surveys"];


            foreach (JObject jo in jArray)
            {
                EDPlanet obj = new EDPlanet();

                if (obj.ParseJson(jo))
                    listObjects.Add(obj);
            }


            return listObjects;
        }


        public List<EDStar> GetAllStars(string system)
        {
            List<EDStar> listObjects = new List<EDStar>();
            string query = "api/v1/star_surveys";

            if (!String.IsNullOrEmpty(system))
                query = query + "/?q[system]=" + HttpUtility.UrlEncode(system);

            var response = RequestGet(query);
            var json = response.Body;

            JArray jArray = null;
            JObject jObject = null;
            if (json != null && json.Length > 5)
                jObject = (JObject)JObject.Parse(json);

            if (jObject == null)
                return listObjects;


            jArray = (JArray)jObject["star_surveys"];


            foreach (JObject jo in jArray)
            {
                EDStar obj = new EDStar();

                if (obj.ParseJson(jo))
                    listObjects.Add(obj);
            }


            return listObjects;
        }


        public bool StorePlanet(EDPlanet edobj)
        {
            
            dynamic jo = new JObject();

            jo.system = edobj.system;
            jo.commander = edobj.commander;
            jo.world = edobj.objectName;
            jo.world_type = edobj.Description;
            jo.terraformable = edobj.terraformable;
            if (edobj.gravity>0)
                jo.gravity = edobj.gravity;
            jo.terrain_difficulty = edobj.terrain_difficulty;
            jo.notes = edobj.notes;

            if (edobj.arrivalPoint>0)
                jo.arrival_point = edobj.arrivalPoint;

            jo.atmosphere_type = edobj.atmosphere.ToString();
            jo.vulcanism_type = edobj.vulcanism.ToString();



            if (edobj.radius>0)
                jo.radius = edobj.radius;

            jo.reserve = edobj.Reserve;
            jo.mass = edobj.mass;
            jo.surface_temp = edobj.surfaceTemp;
            jo.surface_pressure = edobj.surfacePressure;
            jo.orbit_period = edobj.orbitPeriod;
            jo.rotation_period = edobj.rotationPeriod;
            jo.semi_major_axis = edobj.semiMajorAxis;
            jo.rock_pct = edobj.rockPct;
            jo.metal_pct = edobj.metalPct;
            jo.ice_pct = edobj.metalPct;


            jo.carbon = edobj.materials[MaterialEnum.Carbon];
            jo.iron = edobj.materials[MaterialEnum.Iron];
            jo.nickel = edobj.materials[MaterialEnum.Nickel];
            jo.phosphorus = edobj.materials[MaterialEnum.Phosphorus];
            jo.sulphur = edobj.materials[MaterialEnum.Sulphur];
            jo.arsenic = edobj.materials[MaterialEnum.Arsenic];
            jo.chromium = edobj.materials[MaterialEnum.Chromium];
            jo.germanium = edobj.materials[MaterialEnum.Germanium];
            jo.manganese = edobj.materials[MaterialEnum.Manganese];
            jo.selenium = edobj.materials[MaterialEnum.Selenium];
            jo.vanadium = edobj.materials[MaterialEnum.Vanadium];
            jo.zinc = edobj.materials[MaterialEnum.Zinc];
            jo.zirconium = edobj.materials[MaterialEnum.Zirconium];
            jo.cadmium = edobj.materials[MaterialEnum.Cadmium];
            jo.mercury = edobj.materials[MaterialEnum.Mercury];
            jo.molybdenum = edobj.materials[MaterialEnum.Molybdenum];
            jo.niobium = edobj.materials[MaterialEnum.Niobium];
            jo.tin = edobj.materials[MaterialEnum.Tin];
            jo.tungsten = edobj.materials[MaterialEnum.Tungsten];
            jo.antimony = edobj.materials[MaterialEnum.Antimony];
            jo.polonium = edobj.materials[MaterialEnum.Polonium];
            jo.ruthenium = edobj.materials[MaterialEnum.Ruthenium];
            jo.technetium = edobj.materials[MaterialEnum.Technetium];
            jo.tellurium = edobj.materials[MaterialEnum.Tellurium];
            jo.yttrium = edobj.materials[MaterialEnum.Yttrium];

            JObject joPost = new JObject(new JProperty("world_survey", jo));

            if (edobj.id == 0)
            {
                var response = RequestSecurePost(joPost.ToString(), "api/v1/world_surveys");
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    JObject jo2 = (JObject)JObject.Parse(response.Body);
                    JObject obj = (JObject)jo2["world_survey"];
                    edobj.id = obj["id"].Value<int>();
                }
                else if ((int)response.StatusCode == 422)
                {
                    // Surprise, record is already there for some reason!
                    // I may create an api method on the server that negates the need to check for 
                    // this at some point
                    // - Greg

                    var queryParam = $"q[system]={jo.system}&q[world]={jo.world}&q[commander]={jo.commander}";
                    response = RequestGet($"api/v1/world_surveys?{queryParam}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JObject jo2 = (JObject)JObject.Parse(response.Body);
                        JObject obj = (JObject)jo2["world_surveys"][0];
                        edobj.id = obj["id"].Value<int>();

                        response = RequestSecurePatch(joPost.ToString(), "api/v1/world_surveys/" + edobj.id.ToString());
                    }

                }
            }
            else
            {
                var response = RequestSecurePatch(joPost.ToString(), "api/v1/world_surveys/" + edobj.id.ToString());
            }
            return true;
        }

        public bool StoreStar(EDStar edobj)
        {

            dynamic jo = new JObject();

            jo.system = edobj.system;
            jo.commander = edobj.commander;
            jo.star = edobj.objectName;
            jo.star_type = edobj.Description;
            jo.subclass = edobj.subclass;
            jo.solar_mass = edobj.mass;
            jo.solar_radius = edobj.radius;
            jo.star_age = edobj.star_age;
            jo.orbit_period = edobj.orbitPeriod;
            jo.arrival_point = edobj.arrivalPoint;
            jo.luminosity = edobj.luminosity;
            jo.note = edobj.notes;
            jo.surface_temp = edobj.surfaceTemp;

            JObject joPost = new JObject(new JProperty("star_survey", jo));

            if (edobj.id == 0)
            {
                var response = RequestSecurePost(joPost.ToString(), "api/v1/star_surveys");
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    JObject jo2 = (JObject)JObject.Parse(response.Body);
                    JObject obj = (JObject)jo2["star_survey"];
                    edobj.id = obj["id"].Value<int>();
                }
                else if ((int)response.StatusCode == 422)
                {
                    // Surprise, record is already there for some reason!
                    // I may create an api method on the server that negates the need to check for 
                    // this at some point
                    // - Greg

                    var queryParam = $"q[system]={jo.system}&q[star]={jo.star}&q[commander]={jo.commander}";
                    response = RequestGet($"api/v1/star_surveys?{queryParam}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JObject jo2 = (JObject)JObject.Parse(response.Body);
                        JObject obj = (JObject)jo2["star_surveys"][0];
                        edobj.id = obj["id"].Value<int>();

                        response = RequestSecurePatch(joPost.ToString(), "api/v1/star_surveys/" + edobj.id.ToString());
                    }

                }
            }
            else
            {
                var response = RequestSecurePatch(joPost.ToString(), "api/v1/star_surveys/" + edobj.id.ToString());
            }
            return true;


        }



        public bool DeletePlanetID(int id)
        {
            var response = RequestDelete("api/v1/world_surveys/"+id.ToString());
            
            return true;
        }

        public bool Delete(EDPlanet obj)
        {
            if (obj.id > 0)
                return DeletePlanetID(obj.id);

            return true;
        }

    }
}