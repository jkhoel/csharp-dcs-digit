# DCS Digit

DCS Digit automates typing in waypoints and other information to the aircraft's computer, in-place of any working data-cartridge system within DCS.

The program works by simulating a keyboard, reading the internal list of waypoints created within the program and then "pressing" the corresponding key combinations per supported model in order to input these into the aircraft. 

## Creating the list of waypoints manually

There are two options for creating the list of waypoints to be input, one of which is to manually add each waypoint to the list. Waypoints can be rearranged later, but not edited after they are input (future feature).

**IMPORTANT:**
It is important to note that any character required for selecting Northing, leading zeros etc. need to be added also! Example for coordinate N 24 54.432 => `22454432` as the first `2` denotes the N in the case of the F-16C Viper. For the coordinate E 54 23.321 it would be `605423321` with the `6` denoting E, and a leading 0. (These are automatically added if importing from waypoints from a supported format, see below)


## Creating the list of waypoints by importing files

The waypoint list can either be created manually, imported from a *.CF (CombatFlite) file or from a *.JSON file exported from http://mdc.hoelweb.com.

You can add multiple files, or mix manually input and imported waypoints as they are all appended to the list when added, opposed to overwriting.

## Currently supported modules

- F-16C Block 50 by Eagle Dynamics

## Keybinds

The project does not support setting your own keybinds. This means that for this program to work, your will need to set keybinds in DCS keybinds corresponding to the following:

### Module: F-16C Bl.50

| DCS Keybind    | Modifier | Keyboard |
| -------------- | -------- | -------- |
| ICP - 0        | LCtrl    | Num0     |
| ICP - 1        | LCtrl    | Num1     |
| ICP - 2        | LCtrl    | Num2     |
| ICP - 3        | LCtrl    | Num3     |
| ICP - 4        | LCtrl    | Num4     |
| ICP - 5        | LCtrl    | Num5     |
| ICP - 6        | LCtrl    | Num6     |
| ICP - 7        | LCtrl    | Num7     |
| ICP - 8        | LCtrl    | Num8     |
| ICP - 9        | LCtrl    | Num9     |
| ICP - RET      | LCtrl    | Left     |
| ICP - ENT      | LCtrl    | NumEnter |
| ICP DCS - UP   | LCtrl    | Up       |
| ICP DCS - DOWN | LCtrl    | Down     |
