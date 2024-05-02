
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

create view UserView as
	SELECT level, user_name, class_name, weapon_name
            FROM dbo.users as users
            left outer join dbo.class as class
            on users.class_ID = class.class_ID
            left outer join dbo.weapon as weapon
            on users.weapon_ID = weapon.weapon_ID;

create view ClassView as
	SELECT class_name, required_level, default_HP, default_MP
            FROM dbo.class;

create view SkillView as
	SELECT skill_name, element_name, class_name, skill.required_level, damage_coefficient, MP_cost
            FROM dbo.skill as skill
	        inner join dbo.element as element
	        on skill.element_ID = element.element_ID
	        inner join dbo.class as class
	        on skill.required_class_ID = class.class_ID;

create view WeaponTypeView as
	SELECT weapon_type_name, class_name, weaponType.required_level
            FROM dbo.weaponType as weaponType
	        inner join dbo.class as class
	        on weaponType.required_class_ID = class.class_ID;

create view BossView as
	SELECT boss_name, element_name, HP, MP, required_level, required_member
            FROM dbo.Boss as boss
	        inner join dbo.element as element
	        on boss.element_ID = element.element_ID;

create view RaidPartyView as
	SELECT party_name, boss_name, user_name as leader, required_member, current_member
            FROM dbo.raidPartyRoom as raidPartyRoom
	        inner join dbo.boss as boss
	        on raidPartyRoom.boss_ID = boss.boss_ID
	        inner join dbo.users as users
	        on raidPartyRoom.leader_ID = users.user_ID;

create view AuctionView as
	SELECT weapon_name, weapon_type_name, element_name, damage_coefficient, price, user_name as seller
            FROM dbo.auction as auction
	        inner join dbo.weapon as weapon
	        on auction.weapon_ID = weapon.weapon_ID 
	        inner join dbo.weaponType as weaponType
	        on weapon.weapon_type_ID = weaponType.weapon_type_ID
	        inner join dbo.element as element
	        on weapon.element_ID = element.element_ID
	        inner join dbo.users as users
	        on auction.seller_ID = users.user_ID;

create view BanView as
	SELECT users1.user_name as banned_user_name, users2.user_name as GM_name, GM_grade, banned_date, unban_date
            FROM dbo.banList as banList
	        inner join dbo.users as users1
	        on banList.user_ID = users1.user_ID
	        inner join dbo.GM as GM
	        on banList.GM_ID = GM.GM_ID
	        inner join dbo.users as users2
	        on GM.GM_user_ID = users2.user_ID;

create view ElementView as
	select element1.element_name as Name, element2.element_name as Synergy, element3.element_name as Weak
            from dbo.element as element1
	        inner join dbo.element as element2
	        on element1.synergy_element_ID = element2.element_ID
	        inner join dbo.element as element3
	        on element1.weak_element_ID = element3.element_ID;




create role GMRole

grant insert, delete on database::OnlineRPG_DB to GMRole

revoke insert, delete on database::OnlineRPG_DB to kjunwoo234
EXEC sp_droprolemember 'GMRole', 'kjunwoo234';

EXEC sp_addrolemember 'GMRole', 'kjunwoo234';

EXEC sp_droprolemember 'GMRole', 'kjunwoo234';

grant insert, delete on database::OnlineRPG_DB to kjunwoo234
