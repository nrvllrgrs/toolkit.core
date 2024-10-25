# Scoring
`UnityEvaluator` is a data structure that allows for scoring (or filtering). It contains a list of `Evaluators` and `Filters` , each which return a value between 0 and 1. These values are multiplied together to give the final score.

Keep in mind, if the score is ever 0 then the list will immediately stop -- any value times 0 is 0. Because of this, it is recommended that you order your list with performance in mind. For example, LOSFilter (which performs a Raycast) should probably be evaluated later in list.

## Evaluators
Evaluators return a value between 0 and 1.

The following properties are common between all Evaluators:
* **Curve**: Weight curve between range.
* **Bonus Weight**: Bonus weight multiplied to against result, modifying the returning score. It is encouraged that this property is rarely used, otherwise, it can create unintended behaviors.
* **Range**: The contextual value is clamped between the minimum and maximum values.

### Visual Scripting
If an Evaluator does not exist, you can make one via a `ScriptMachine`. To create one, perform the following steps:

 1. Add a `ScriptMachine` to the gameObject.
 2. Add a `ScriptMachineEvaluator` to your scorer.
 3. Drag-and-drop the ScriptMachine component onto its *Script Machine* property.
 4. Enter a string into its *Event Name* property.
 5. Edit the `Script Machine`'s graph.
 6. Add an Evaluator Event unit and enter the same string to its Event Name.
 7. Perform the desired calculations and return that result by setting the *value* of the `ScriptMachineEvaluator`.

It is recommended that ScriptMachineEvaluators are not used for production. They are great for quick experiments.

## Filters
Filters return either 0 (false) or 1 (true).

The following properties are common between all Filters:
* **Override or Skip**: Indicates whether this true condition forces a score of 1; whereas, a false condition is skipped.

> This property can be useful when determining whether an AI *always* detects the player within a  given distance -- regardless of other conditions.

* **Inverted**: Indicates whether filter result is inverted. In other words, false becomes true and true becomes false.
* **Bonus Weight**: Bonus weight multiplied to against result, modifying the returning score. It is encouraged that this property is rarely used, otherwise, it can create unintended behaviors.

### Visual Scripting
If a Filter does not exist, you can make one via a `ScriptMachine`. To create one, perform the following steps:

 1. Add a `ScriptMachine` to the gameObject.
 2. Add a `ScriptMachineFilter` to your scorer.
 3. Drag-and-drop the ScriptMachine component onto its *Script Machine* property.
 4. Enter a string into its *Event Name* property.
 5. Edit the `Script Machine`'s graph.
 6. Add an Evaluator Event unit and enter the same string to its Event Name.
 7. Perform the desired calculations and return that result by setting the *isIncluded* of the `ScriptMachineFilter`.

It is recommended that ScriptMachineFilters are not used for production. They are great for quick experiments.