
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

	drop view userview

create view UserView as
	SELECT RANK() over (order by level desc) as user_rank, level, user_name, class_name, weapon_name
            FROM dbo.users as users
            left outer join dbo.class as class
            on users.class_ID = class.class_ID
            left outer join dbo.weapon as weapon
            on users.weapon_ID = weapon.weapon_ID;

--select * from userview

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



if OBJECT_ID('RefreshTable') is not null
	drop procedure dbo.RefreshTable;
go

create procedure RefreshTable
as
	begin
	delete from class

	insert into class values ('001', 'Sword Man', 11, 100, 100);
	insert into class values ('002', 'Archer', 9, 90, 110);
	insert into class values ('003', 'Mage', 8, 60, 140);
	insert into class values ('004', 'Assassin', 10, 80, 20);
	insert into class values ('005', 'Priest', 6, 100, 120);

	--select * from class


	delete from element

	insert into element values ('001', 'Fire', '002', '003');
	insert into element values ('002', 'Poison', '001', '004');
	insert into element values ('003', 'Water', '004', '006');
	insert into element values ('004', 'Ice', '003', '005');
	insert into element values ('005', 'Wind', '006', '002');
	insert into element values ('006', 'Nature', '005', '001');
	insert into element values ('007', 'Dark', '008', '009');
	insert into element values ('008', 'Undead', '007', '010');
	insert into element values ('009', 'Light', '010', '007');
	insert into element values ('010', 'Holy', '009', '008');

	--select * from element


	delete from skill;

	insert into skill values ('001', 'Fire Slash', '001', 17, 1.2, '001', 20);
	insert into skill values ('002', 'Frozen Arrow', '002', 21, 1.5, '004', 30);
	insert into skill values ('003', 'Dust Tornado', '003', 25, 2.4, '005', 55);
	insert into skill values ('004', 'Shadow Assault', '004', 30, 3.7, '007', 70);
	insert into skill values ('005', 'Bless', '005', 13, 0.8, '010', 15);
	insert into skill values ('006', 'Spore Charm', '004', 22, 1.3, '002', 22);

	--select * from skill


	delete from weaponType;

	insert into weaponType values ('001', 'Big Sword', '001', 8);
	insert into weaponType values ('002', 'Rapier', '001', 35);
	insert into weaponType values ('003', 'Crossbow', '002', 6);
	insert into weaponType values ('004', 'Long Bow', '002', 27);
	insert into weaponType values ('005', 'Magic Wand', '003', 5);
	insert into weaponType values ('006', 'Mystic Ball', '003', 43);
	insert into weaponType values ('007', 'Dagger', '004', 7);
	insert into weaponType values ('008', 'Shuriken', '004', 15);
	insert into weaponType values ('009', 'Bible', '005', 5);
	insert into weaponType values ('010', 'Cross Staff', '005', 35);
	
	--select * from weaponType


	delete from weapon;

	insert into weapon values ('001', 'Megasonic Thunder Rapier', '002', '009', 4.3);
	insert into weapon values ('002', 'Twirling Wind Breaker', '004', '006', 2.7);
	insert into weapon values ('003', 'Forbidden Elder Wand', '005', '007', 3.1);
	insert into weapon values ('004', 'Throwable Tasty Cheeseball', '008', '001', 5.4);
	insert into weapon values ('005', 'The Literally King''s Cross', '010', '003', 1.8);
	insert into weapon values ('006', 'Wook Stick', '005', '006', 0.6);

	--select * from weapon


	delete from users;

	insert into users values ('001', 'GM1', -1, NULL, NULL);
	insert into users values ('002', 'GM2', -1, NULL, NULL);
	insert into users values ('003', 'kjunwoo23', 48, '004', '004');
	insert into users values ('004', 'IWoN''tHeAlyOu', 35, '005', '005');
	insert into users values ('005', 'iStartedyesterday', 9, '003', '006');
	insert into users values ('006', 'GM3', -1, NULL, NULL);

	--select * from users


	delete from userSkills;

	insert into userSkills values ('003', '004');
	insert into userSkills values ('004', '005');
	insert into userSkills values ('003', '006');

	--select * from userSkills


	delete from GM;

	insert into GM values ('001', '001', 'HeadGM');
	insert into GM values ('002', '002', 'Staff');
	insert into GM values ('003', '006', 'Staff');

	--select * from GM


	delete from banList;

	insert into banList values ('001', '004', '001', '2024-04-12 04:47:22', '2024-04-18 04:47:22');

	--select * from banList


	delete from auction;

	insert into auction values ('001', '002', '003', 15000);
	insert into auction values ('002', '003', '004', 27000);

	--select * from auction


	delete from boss;

	insert into boss values ('001', 'Elsa and Olaf', '004', 500, 20, 5, 4);
	insert into boss values ('002', 'Fenrir', '006', 5000, 200, 40, 8);

	--select * from boss


	delete from raidPartyRoom;

	insert into raidPartyRoom values ('001', 'anybody need olaf carrot?', '001', '005', 2);
	insert into raidPartyRoom values ('002', 'fire element welcome', '002', '003', 3);
	insert into raidPartyRoom values ('003', 'let me raid boss plz', '002', '004', 1);

	--select * from raidPartyRoom
	end

go

exec RefreshTable



if OBJECT_ID('TestTable') is not null
	drop table dbo.TestTable;
go

create table TestTable
	(
	test_ID		varchar(3) primary key,
	test_name	varchar(20),
	test_num	numeric(3, 0)
	);

insert into TestTable values ('kjunwoo23', 123);
insert into TestTable values ('kjunwoo234', 234);

select * from TestTable
