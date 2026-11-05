
## Correction behavior

- `boost, no, shield`: only shield executes.
- `boost`, followed within five real-time seconds by `sorry, I meant siren`:
  the boost action is reversed where possible and siren activates.
- `shield instead of boost`, `actually shield`, `make that shield`, and
  `shield instead` also express corrections.
- `no` alone asks for the replacement; the next command completes the correction
  within the window. The original effect continues until a valid replacement is
  available.
- An ordinary `shield` after `boost` is a new command, so both effects can remain
  active. Correction requires explicit wording or an outstanding clarification.
- Repeating the same command as a correction confirms it without toggling again.
- An expired correction asks the player to repeat the desired command. A full
  inline correction can still be understood without previous command history.
- Used charge, elapsed movement, and collisions are never refunded or rewound.
  Replacements respect existing cooldowns; unavailable replacements leave the
  previous effect in place.
- The window uses unscaled time, so it expires even while the game is paused.
  The `correctionWindowSeconds` field on VoiceInput defaults to 5.
- Each recognition result selects one action from ranked speech alternatives.
  Ambiguous phrases containing multiple actions ask the player to clarify.
- The menu also parses inline corrections before starting a race. A scene
  transition that has already happened cannot be undone by a later correction.

## Android smoke test

Use an Android device with the game's normal microphone setup. Activate boost,
then say `sorry, I meant shield` promptly. Check that boost stops, shield starts,
and the feedback says `Corrected: Boost -> Shield`. Repeat with an inline
correction, an unavailable power-up, and a correction after five seconds.
Also pause by voice and correct it to a power-up within five seconds; the game
should resume. The regression suite validates received transcript handling;
only a device test verifies what the speech service actually hears.
