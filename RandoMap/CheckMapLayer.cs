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

        private void AddMarkers(On.PlayerLocation.orig_OnEnable orig, PlayerLocation self)
        {
            orig(self);

            if (!MapEnabled())
            {
                return;
            }

            var helper = HelperLog.Generate();
            if (helper == null)
            {
                return;
            }
            var mapTransform = self.mapScreen;
            var markerTemplate = mapTransform.gameObject.GetComponentsInChildren<Marker>()
                .Where(m => m.powercell).Select(m => m.gameObject).FirstOrDefault();
            if (markerTemplate == null)
            {
                RandoMapPlugin.LogError("power cell marker not found");
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
                    checkMarker = UE.GameObject.Instantiate(markerTemplate);
                    checkMarker.transform.parent = self.locationRect.parent;
                    var img = checkMarker.GetComponent<UI.Image>();
                    img.sprite = checkSprite;
                    var rtransform = checkMarker.GetComponent<UE.RectTransform>();
                    rtransform.parent = self.locationRect.parent;
                    rtransform.anchorMax = self.locationRect.anchorMax;
                    rtransform.anchorMin = self.locationRect.anchorMin;
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

        public void Hook()
        {
            On.PlayerLocation.OnEnable += AddMarkers;
        }
    }
}