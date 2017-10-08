Fork from [github.com/Microsoft/cci](https://github.com/BrokenEvent/BrokenEvent.PDBReader/blob/master/Microsoft.Cci.License.txt)

# BrokenEvent.PDBReader

## About This Fork

The goal is creation of a convenient interface for PDB reading and filename:line resolving. Microsoft.cci.pdb supports it, but the features are hidden with strange interface and partially commented.
Now almost whole Microsoft code is cut of the build as it is not related to the this functionality.

## Features

* Simple and lightweight
* Filename:line resolve by classname and the method name
* More precise filename:line resolve if IL offset provided (can be found in ``StackFrame.GetILOffset()``)

## Usage

* Attach ``BrokenEvent.PDBReader assembly`` to project
* Use namespace ``BrokenEvent.PDBReader assembly``
* Create ``PdbResolver`` using stream or filename constructor
* Resolve required code location with ``pdbResolver.FindLocation("MyNamespace.MyClass", "MyMethod"/*, optional IL offset*/)``

## License

Whole project is published with under MIT license.

Core of cci.pdb:
© Microsoft. See [Microsoft.Cci.License.txt](https://github.com/BrokenEvent/BrokenEvent.PDBReader/blob/master/Microsoft.Cci.License.txt)

Cut of and simplification:
©2017, BrokenEvent.
