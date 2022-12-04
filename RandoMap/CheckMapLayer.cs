using Rando = Haiku.Rando;
using RChecks = Haiku.Rando.Checks;
using RTopology = Haiku.Rando.Topology;
using UE = UnityEngine;
using UI = UnityEngine.UI;
using USM = UnityEngine.SceneManagement;
using Collections = System.Collections.Generic;
using static System.Linq.Enumerable;

namespace RandoMap
{
    internal class CheckMapLayer
    {
        public System.Func<bool> MapEnabled = () => false;

        private readonly Collections.List<string> sceneNamesById;

        private readonly Collections.Dictionary<RTopology.RandoCheck, UE.GameObject> markers = new();

        private UE.GameObject? markerTemplate;

        private UE.GameObject? markerLegend;

        public CheckMapLayer()
        {
            var numScenes = USM.SceneManager.sceneCountInBuildSettings;
            sceneNamesById = new(numScenes);
            for (var i = 0; i < numScenes; i++)
            {
                var name = USM.SceneUtility.GetScenePathByBuildIndex(i);
                var lastSlash = name.LastIndexOf('/');
                if (lastSlash != -1)
                {
                    name = name.Substring(lastSlash + 1);
                }
                if (name.EndsWith(".unity")) {
                    name = name.Substring(0, name.Length - 6);
                }
                sceneNamesById.Add(name);
            }
        }

        private void AddMarkersOnOpenMap(On.PlayerLocation.orig_OnEnable orig, PlayerLocation self)
        {
            orig(self);
            try
            {
                AddMarkers(self);
            }
            catch (Exception err)
            {
                RandoMapPlugin.LogError(err.ToString());
            }
        }

        private UE.GameObject? GetMarkerTemplate(PlayerLocation self)
        {
            if (markerTemplate == null)
            {
                RandoMapPlugin.LogInfo("CheckMapLayer: getting power cell marker");
                var powercellMarker = self.mapScreen.gameObject.GetComponentsInChildren<Marker>()
                    .Where(m => m.powercell).Select(m => m.gameObject).FirstOrDefault();
                if (powercellMarker == null)
                {
                    RandoMapPlugin.LogError("power cell marker not found");
                    return null;
                }
                // This intermediate instantiation is needed so that actual markers instantiated
                // later appear immediately.
                markerTemplate = UE.GameObject.Instantiate(powercellMarker);
                UE.GameObject.Destroy(markerTemplate.GetComponent<Marker>());
                markerTemplate.SetActive(false);
            }
            return markerTemplate;
        }

        private void AddMarkers(PlayerLocation self)
        {
            if (!MapEnabled())
            {
                foreach (var m in markers.Values)
                {
                    m.SetActive(false);
                }
                return;
            }
            var helper = HelperLog.Generate();
            if (helper == null)
            {
                return;
            }
            var mapTransform = self.mapScreen;
            var template = GetMarkerTemplate(self);
            if (template == null)
            {
                return;
            }
            var shownChecks = helper.ReachableUnvisitedChecks();
            // Show checks that are reachable and unvisited, and hide those that are not.
            foreach (var ent in markers)
            {
                if (!shownChecks.Contains(ent.Key))
                {
                    ent.Value.SetActive(false);
                }
            }
            var checkSprite = BundledSprites.Get("Check Marker.png");
            // self.locationRect isn't set the first time that OnEnable is called;
            // so we must get that component ourselves.
            var locationRect = self.GetComponent<UE.RectTransform>();
            foreach (var rc in shownChecks)
            {
                var roomName = sceneNamesById[rc.SceneId];
                var room = self.rooms.Where(r => r.name == roomName).FirstOrDefault();
                if (room == null)
                {
                    RandoMapPlugin.LogError($"CheckMapLayer: room {roomName} not found in map");
                    continue;
                }
                var roomTransform = room.GetComponent<UE.RectTransform>();
                if (!markers.TryGetValue(rc, out var checkMarker))
                {
                    checkMarker = UE.GameObject.Instantiate(template);
                    checkMarker.transform.parent = locationRect.parent;
                    var img = checkMarker.GetComponent<UI.Image>();
                    img.sprite = checkSprite;
                    // important in case the template marker was hidden
                    img.enabled = true;
                    var rtransform = checkMarker.GetComponent<UE.RectTransform>();
                    rtransform.parent = locationRect.parent;
                    rtransform.anchorMax = locationRect.anchorMax;
                    rtransform.anchorMin = locationRect.anchorMin;
                    rtransform.anchoredPosition = new UE.Vector2(
                        UE.Mathf.Round(roomTransform.anchoredPosition.x) + UE.Mathf.Round(rc.Position.x),
                        UE.Mathf.Round(roomTransform.anchoredPosition.y) + UE.Mathf.Round(rc.Position.y)
                    );
                    markers[rc] = checkMarker;
                    var destructor = checkMarker.AddComponent<Destructor>();
                    destructor.enabled = true;
                    destructor.Func = () => markers.Remove(rc);
                }
                checkMarker.SetActive(true);
            }
        }

        private void ModMarkerLegends(On.MarkerLegend.orig_ShowLegend orig, MarkerLegend self)
        {
            orig(self);
            try
            {
                
                if (MapEnabled())
                {
                    // If power cells are randomized, the legend for their markers should not appear.
                    if (PowercellsAreRandomized())
                    {
                        self.powercell.SetActive(false);
                    }
                    var ml = GetMarkerLegend(self.powercell);
                    ml.SetActive(RChecks.CheckManager.Instance.Randomizer != null);
                }
            }
            catch (Exception err)
            {
                RandoMapPlugin.LogError(err.ToString());
            }
        }

        private UE.GameObject GetMarkerLegend(UE.GameObject template)
        {
            if (markerLegend != null)
            {
                return markerLegend;
            }
            markerLegend = UE.GameObject.Instantiate(template);
            markerLegend.transform.parent = template.transform.parent;
            var img = markerLegend.GetComponentInChildren<UI.Image>();
            img.sprite = BundledSprites.Get("Check Marker.png");
            var txt = markerLegend.GetComponentInChildren<TranslateTextMPro>();
            txt.GetComponent<TMPro.TMP_Text>().text = Text._RANDO_CHECK;
            var destructor = markerLegend.AddComponent<Destructor>();
            destructor.Func = () => markerLegend = null;
            destructor.enabled = true;
            return markerLegend;
        }

        private void HidePowercellMarkers(On.Marker.orig_ShowPowercell orig, Marker self)
        {
            orig(self);
            try
            {
                // If power cells are randomized, their markers should not appear.
                if (self.powercell && MapEnabled() && PowercellsAreRandomized())
                {
                    self.image.enabled = false;
                }
            }
            catch (Exception err)
            {
                RandoMapPlugin.LogError(err.ToString());
            }
        }

        private static bool PowercellsAreRandomized() => 
            RChecks.CheckManager.Instance.Randomizer != null &&
            Rando.Settings.IncludePowerCells.Value;

        public void Hook()
        {
            On.PlayerLocation.OnEnable += AddMarkersOnOpenMap;
            On.MarkerLegend.ShowLegend += ModMarkerLegends;
            On.Marker.ShowPowercell += HidePowercellMarkers;
        }
    }
}