# Format translation

## Motivation
Suppose we want to display how simulated game flows. This surely needs evaluation
structures (for bots evaluation), but it also needs standard game objects that
are displayed.

Those are two types of structures that are not same e.g. *Region* does have *Name*
field, but *RegionMin* (its mirror for evaluating) does not need anything like that.

## Approach
Simplest approach will be to have IDs on every class of standard game objects. This ID
unique will have to be unique.

ID will be used as a mean to map evaluation structures and game objects.

For example (simplified): Suppose we have *Region* game object with fields *Name, OwningPlayer,
Id*. We can easily map this to *RegionMin*, because *OwningPlayer* is of type *Player*,
who has also ID. Thus *RegionMin*'s fields will only have to have fields *ID, OwningPlayerId*.

Similar approach can be used for all other classes.

## Translation
Translation itself will be handled by *ObjectTranslationHandler*. This class
will have methods that will take structure from one format and translate it into another
(and the other way).

This approach is better than having methods like *ToMinifiedRegion()* or *FromMinifiedRegion*,
because this way *Region* doesn't need to know about existence of *MinRegion* or 
*MinRegion* doesn't need to know about *Region*.

## Neutral format objects
*What format should we use, if the class is not for evaluation purpose and isn't used as big game object?*

1. **Game object format** - inappropriate, too much useless fields (Name,...)
2. **Minified format** - possible, contains all the information needed (though compressed)
3. **Neutral format** - possible, but there will be new structures similar to those
in minified format (just not that optimized)
4. **Interface** - a good option would be to hide every game object
behind an interface. It is not possible, because not all fields are same (neighbours).

3rd option seems to be the best one, because there are classes that we need,
but that aren't and have no reason to be in evaluation structures e.g. *Turn*.