SELECT user_name, level, class_name, weapon_name
FROM dbo.users as users
	left outer join dbo.class as class
	on users.class_ID = class.class_ID 
	left outer join dbo.weapon as weapon
	on users.weapon_ID = weapon.weapon_ID
		
where level between 0 and 10 and users.user_name not in
		(select user_name
		from dbo.users as users
		left outer join dbo.banList as banList
		on users.user_ID = banList.user_ID
		left outer join dbo.GM as GM
		on users.user_ID = GM.GM_user_ID
		where users.user_ID = banList.user_ID or users.user_ID = gm.GM_user_ID)

	order by level desc

SELECT class_name, required_level, default_HP, default_MP
FROM dbo.class

SELECT skill_name, element_name, class_name, skill.required_level, damage_coefficient as damage, MP_cost
FROM dbo.skill as skill
	inner join dbo.element as element
	on skill.element_ID = element.element_ID
	inner join dbo.class as class
	on skill.required_class_ID = class.class_ID 


SELECT weapon_type_name, class_name, weaponType.required_level
FROM dbo.weaponType as weaponType
	inner join dbo.class as class
	on weaponType.required_class_ID = class.class_ID

SELECT boss_name, element_name, HP, MP, required_level, required_member
FROM dbo.Boss as boss
	inner join dbo.element as element
	on boss.element_ID = element.element_ID


SELECT party_name, boss_name, user_name as leader, required_member, current_member
FROM dbo.raidPartyRoom as raidPartyRoom
	inner join dbo.boss as boss
	on raidPartyRoom.boss_ID = boss.boss_ID
	inner join dbo.users as users
	on raidPartyRoom.leader_ID = users.user_ID	
	where users.user_name not in
		(select user_name
		from dbo.users as users
		inner join dbo.banList as banList
		on users.user_ID = banList.user_ID)



SELECT weapon_name, weapon_type_name, element_name, damage_coefficient as damage, price, user_name as seller
FROM dbo.auction as auction
	inner join dbo.weapon as weapon
	on auction.weapon_ID = weapon.weapon_ID 
	inner join dbo.weaponType as weaponType
	on weapon.weapon_type_ID = weaponType.weapon_type_ID
	inner join dbo.element as element
	on weapon.element_ID = element.element_ID
	inner join dbo.users as users
	on auction.seller_ID = users.user_ID	
	where users.user_name not in
		(select user_name
		from dbo.users as users
		inner join dbo.banList as banList
		on users.user_ID = banList.user_ID)


SELECT users1.user_name as banned_user_name, users2.user_name as GM_name, GM_grade, banned_date, unban_date
FROM dbo.banList as banList
	inner join dbo.users as users1
	on banList.user_ID = users1.user_ID
	inner join dbo.GM as GM
	on banList.GM_ID = GM.GM_ID
	inner join dbo.users as users2
	on GM.GM_user_ID = users2.user_ID
	where users1.user_name like '%hel%'

select element1.element_name as Name, element2.element_name as Synergy, element3.element_name as Weak
from dbo.element as element1
	inner join dbo.element as element2
	on element1.synergy_element_ID = element2.element_ID
	inner join dbo.element as element3
	on element1.weak_element_ID = element3.element_ID

select *
from dbo.users

select required_member
from dbo.boss as boss
	where boss.boss_ID = '002'

