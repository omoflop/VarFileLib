# VarFileLib
A goofy little library for a (very objectively and not subjectively) better then JSON data format

### Example syntax:
```
[Name]="Joe"
[Path]="joe"
[Portraits]="default"

[WalkAnim]={
	[Up]=speed:10, frames:4, img:"walk_up",
	[Down]=speed:10, frames:4, img:"walk_down",
	[Left]=speed:10, frames:5, img:"walk", flip:true,
	[Right]=speed:10, frames:5, img:"walk",
}

[IdleAnim]={
	[Up]=frames:1, img:"idle_up",
	[Down]=frames:1, img:"idle_down",
	[Left]=frames:1, img:"idle", flip:true,
	[Right]=frames:1, img:"idle",
}

[Hitbox]=width:16, height:8,
[InteractionBox]=width:16, height:16,
```

For syntax highlighting, use the provided nodepad++ language
