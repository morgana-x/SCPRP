# SCP DarkRP ![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/morgana-x/SCPRP/total)

Highly configurable Jobs, Purchasable items and micellaneous options

Requires a MySQL server, if running on windows MySQL Workbench is a nice easy utility.

# Getting started

Type `.rphelp` in the in-game client console for a list of commands

Ensure that you manually bind the buy door keybind in server specific settings when joining the game


# Installation

Copy `DarkRP.dll` into `LabAPI\plugins\global` or `LabAPI\plugins\XXXX`

Extract `dependencies.zip` into `LabAPI\dependencies\global` or `LabAPI\dependencies\XXXX`

Start the server (It will fail to load the plugin initially)

Configure the generated `LabAPI\configs\XXXX\DarkRP\Modules\Database.yml` to match your SQL servers details

Restart the server


## Smarter setup
If you only want to start the server once:

Create the `Database.yml` ahead of time (in the path mentioned above) and format it like this

```yaml
# SQL Server IP address/domain
ip: localhost
# SQL Server port
port: 3306
# SQL Server Username
user: root
# SQL Server Password
pw: root
# SQL Server Database
db: scpsl_rp
# SQL Server Table (Will auto create a table of this name)
table: rp_money
```