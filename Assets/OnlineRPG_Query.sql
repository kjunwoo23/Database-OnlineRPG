SELECT user_name, level, class_name, weapon_name
FROM dbo.users as users
	left outer join dbo.class as class
	on users.class_ID = class.class_ID 
	left outer join dbo.weapon as weapon
	on users.weapon_ID = weapon.weapon_ID
	order by level desc

SELECT class_name, required_level, default_HP, default_MP
FROM dbo.class

SELECT skill_name, element_name, class_name, skill.required_level, damage_coefficient, MP_cost
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


SELECT party_name, boss_name, user_name, required_member, current_member
FROM dbo.raidPartyRoom as raidPartyRoom
	inner join dbo.boss as boss
	on raidPartyRoom.boss_ID = boss.boss_ID
	inner join dbo.users as users
	on raidPartyRoom.leader_ID = users.user_ID



--auction때 재사용
SELECT weapon_name, weapon_type_name, element_name, damage_coefficient, price, user_name
FROM dbo.auction as auction
	inner join dbo.weapon as weapon
	on auction.weapon_ID = weapon.weapon_ID 
	inner join dbo.weaponType as weaponType
	on weapon.weapon_type_ID = weaponType.weapon_type_ID
	inner join dbo.element as element
	on weapon.element_ID = element.element_ID
	inner join dbo.users as users
	on auction.seller_ID = users.user_ID