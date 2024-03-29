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
                // The train room has no singular location on the map.
                if (rc.SceneId == Rando.SpecialScenes.Train)
                {
                    continue;
                }
                var roomName = sceneNamesById[rc.SceneId];
                var room = self.rooms.Concat(self.staticNPCRooms).Where(r => r.name == roomName).FirstOrDefault();
                if (room == null)
                {
                    RandoMapPlugin.LogError($"CheckMapLayer: room {rc.SceneId} ({roomName}) not found in map");
                    continue;
                }
                var roomPos = room.GetComponent<UE.RectTransform>().anchoredPosition;
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
                        UE.Mathf.Round(roomPos.x) + UE.Mathf.Round(rc.Position.x),
                        UE.Mathf.Round(roomPos.y) + UE.Mathf.Round(rc.Position.y)
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
                // If power cells are randomized, the legend for their markers should not appear.
                if (MapEnabled() && PowercellsAreRandomized())
                {
                    self.powercell.SetActive(false);
                }
                SetMarkerLegend(self.healthStation, MapEnabled() && RChecks.CheckManager.Instance.Randomizer != null);
            }
            catch (Exception err)
            {
                RandoMapPlugin.LogError(err.ToString());
            }
        }

        private void SetMarkerLegend(UE.GameObject template, bool state)
        {
            if (!state)
            {
                markerLegend?.SetActive(false);
                return;
            }
            if (markerLegend == null)
            {
                markerLegend = UE.GameObject.Instantiate(template, template.transform.parent);
                UI.LayoutRebuilder.ForceRebuildLayoutImmediate(markerLegend.GetComponent<UE.RectTransform>());
                var img = markerLegend.GetComponentInChildren<UI.Image>();
                img.sprite = BundledSprites.Get("Check Marker.png", markerLegendPPU);
                var txt = markerLegend.GetComponentInChildren<TranslateTextMPro>();
                txt.GetComponent<TMPro.TMP_Text>().text = Text._RANDO_CHECK;
                var destructor = markerLegend.AddComponent<Destructor>();
                destructor.Func = () => markerLegend = null;
                destructor.enabled = true;
            }
            markerLegend.SetActive(true);
        }

        private const float markerLegendPPU = 16;

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

        private static bool PowercellsAreRandomized()
        {
            var r = RChecks.CheckManager.Instance.Randomizer;
            return r != null && r.Settings.Contains(Rando.Pool.PowerCells);
        }

        public void Hook()
        {
            On.PlayerLocation.OnEnable += AddMarkersOnOpenMap;
            On.MarkerLegend.ShowLegend += ModMarkerLegends;
            On.Marker.ShowPowercell += HidePowercellMarkers;
        }
    }
}