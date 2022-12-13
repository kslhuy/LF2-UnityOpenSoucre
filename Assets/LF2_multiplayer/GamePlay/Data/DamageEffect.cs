namespace LF2{

	public enum DamageEffect{
		Normal, //Effect: 0 , Sound 001/006
		Blood, //Effect: 1 , Sound : 032/033
		
		Fire, // Effect: 2 , Sound : 070/071
		Ice, // Effect: 3 , Sound : 065/066
		weapon_hit_sound, // Effect: 4 , Sound : weapon_hit_sound
		AmorHit,// Effect: 5 , Sound : 085
		BlockPunch,// Effect: 6 , Sound : 002

		NoEffect, // Effect: 7 ,   InAir
	}

/* Effect: 0
Sound: 001/006.wav
Effect: Normal, weapons fly away
Ex.: [can leave it out]
itrs without any effect act like this
Effect: 1
Sound: 032/033.wav
Effect: Blood, weapons fly away
Ex.: Deep's sword attacks
-----------
Effect: 2
Sound: 070/071.wav
Effect: Fire, can hit burning characters, weapons fly away
Ex.: Firen's Firerun
-----------

Effect: 3
Sound: 065/066.wav
Effect: Ice, can hit frozen characters, weapons fly away
Ex.: Ice Ball
-----------

Effect: 4
Sound: weapon_hit_sound
Effect: Reflect all flying attacks with state: 3000, weapons fly away, has no influence on other characters
Ex.: "Shrafe" Attacks
-----------

Effect: 5
Sound: [no Sound]
Effect: Reflects all flying attacks with state: 3000, weapons fly away, characters are hit without sound
Ex.: [Never used]
Any effect not listed acts like this
-----------

Effect: 20
Sound: 070/071.wav
Effect: Fire, burning characters are immune to effect 20/21, weapons don't fly away
Ex.: "Fire" frames, Firen's Ground-Fire
-----------

Effect: 21
Sound: 070/071.wav
Effect: Fire, burning characters are immune to effect 20/21, weapons fly away, will not hit teammates when combined with state: 18
Ex.: Firen's Inferno
-----------

Effect: 22
Sound: 070/071.wav
Effect: Fire, can hit burning characters, weapons fly away, a positive dvx goes to the middle, will not hit teammates when combined with state: 18
Ex.: Firen's Explosion
-----------

Effect: 23
Sound: 070/071.wav
Effect: Normal, character is hurtable, weapons fly away, a positive dvx goes to the middle
Ex.: Julian's Explosion
-----------

Effect: 30
Sound: 065.wav
Effect: Ice, frozen characters are immune to effect: 30, weapons fly away
Ex.: Freeze Icicle
 */
		


}
