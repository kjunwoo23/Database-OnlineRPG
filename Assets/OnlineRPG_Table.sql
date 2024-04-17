
create table class
	(
	 class_ID				varchar(3),
	 class_name				varchar(20),
	 required_level			numeric(5,0),
	 default_HP				numeric(5,0),
	 default_MP				numeric(5,0),
	 primary key (class_ID)
	);

create table element
	(
	 element_ID				varchar(3),
	 element_name			varchar(20),
	 synergy_element_ID		varchar(3),
	 weak_element_ID		varchar(3),
	 primary key (element_ID)
	);

create table skill
	(
	 skill_ID				varchar(3),
	 skill_name				varchar(30),
	 required_class_ID		varchar(3),
	 required_level			numeric(5,0),
	 damage_coefficient		numeric(7,3),
	 element_ID				varchar(3),
	 MP_cost				numeric(5,0),
	 primary key (skill_ID),
	 foreign key (required_class_ID) references class
		on delete set null,
	 foreign key (element_ID) references element
		on delete set null
	);

create table weaponType
	(
	 weapon_type_ID			varchar(3),
	 weapon_type_name		varchar(20),
	 required_class_ID		varchar(3),
	 required_level			numeric(5,0),
	 primary key (weapon_type_ID),
	 foreign key (required_class_ID) references class
		on delete set null
	);

create table weapon
	(
	 weapon_ID				varchar(10),
	 weapon_name			varchar(30),	--weapon_name can be customized by users
	 weapon_type_ID			varchar(3),
	 element_ID				varchar(3),
	 damage_coefficient		numeric(7,3),
	 primary key (weapon_ID),
	 foreign key (weapon_type_ID) references weaponType
		on delete set null,
	 foreign key (element_ID) references element
		on delete set null
	);

create table users
	(
	 user_ID				varchar(5),
	 user_name				varchar(30),
	 level					numeric(5,0),
	 class_ID				varchar(3),
	 weapon_ID				varchar(10),
	 primary key (user_ID),
	 foreign key (class_ID) references class
		on delete set null,
	 foreign key (weapon_ID) references weapon
		on delete set null
	);

create table userSkills
	(
	 user_ID				varchar(5),
	 skill_ID				varchar(3),
	 primary key (user_ID, skill_ID),
	 foreign key (user_ID) references users,
	 foreign key (skill_ID) references skill
		on delete CASCADE
	);

create table GM
	(
  	 GM_ID					varchar(3),
	 GM_user_ID				varchar(5),
	 GM_grade				varchar(10),
	 primary key (GM_ID),
	 foreign key (GM_user_ID) references users
		on delete set null
	);

create table banList
	(
	 ban_ID					varchar(5),
	 user_ID				varchar(5),
	 GM_ID					varchar(3),
	 banned_date			datetime2,
	 unban_date				datetime2,
	 primary key (ban_ID),
	 foreign key (user_ID) references users
		on delete set null,
	 foreign key (GM_ID) references GM
		on delete set null
	);

create table auction
	(
	 auction_ID				varchar(5),
	 weapon_ID				varchar(10),
	 seller_ID				varchar(5),
	 price					numeric(10,0),
	 primary key (auction_ID),
	 foreign key (weapon_ID) references weapon
		on delete set null,
	 foreign key (seller_ID) references users
		on delete set null
	);

create table boss
	(
	 boss_ID				varchar(3),
	 boss_name				varchar(20),
	 element_ID				varchar(3),
	 HP						numeric(10,3),
	 MP						numeric(10,3),
	 required_level			numeric(5,0),
	 required_member		numeric(3,0),
	 primary key (boss_ID),
	 foreign key (element_ID) references element
		on delete set null
	);

create table raidPartyRoom
	(
	 party_ID				varchar(10),
	 party_name				varchar(30),	--party_name can be customized by users
	 boss_ID				varchar(3),
	 leader_ID				varchar(5),
	 current_member			numeric(3,0),
	 primary key (party_ID),
	 foreign key (boss_ID) references boss
		on delete set null,
	 foreign key (leader_ID) references users
		on delete set null
	);