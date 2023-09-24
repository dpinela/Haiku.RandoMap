# 1.1.1 (24 September 2023)

Fixes a bug that caused the map and helper log to not function with
Randomizer 2.2.3 or later.

# 1.1 (30 August 2023)

Due to changes in the Randomizer's implementation (supporting the performance improvements
in rando and this mod), this version of RandoMap requires Randomizer 2.2.2 or later.

Enhancements:

- Searching for reachable checks is now much faster; as a result, opening the map no longer
  freezes the game momentarily when there are many checks available.

Bug fixes for the spoiler log:

- Scrap piles that were consolidated away no longer appear.
- All rooms that contain non-unique locations (such as scrap piles) now have defined names
  and no longer appear as "???".
- Lever locations each have their own name and are thus considered unique and do not have a
  room name in the log.