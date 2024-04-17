
--delete from weaponType
--delete from skill;


delete from class;

insert into class values ('001', 'Sword Man', 11, 100, 100);
insert into class values ('002', 'Archer', 9, 90, 110);
insert into class values ('003', 'Mage', 8, 60, 140);
insert into class values ('004', 'Assassin', 10, 80, 20);
insert into class values ('005', 'Priest', 6, 100, 120);

select * from class


delete from element;

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

select * from element


delete from skill;

insert into skill values ('001', 'Fire Slash', '001', 17, 1.2, '001', 20);
insert into skill values ('002', 'Frozen Arrow', '002', 21, 1.5, '004', 30);
insert into skill values ('003', 'Dust Tornado', '003', 25, 2.4, '005', 55);
insert into skill values ('004', 'Shadow Assault', '004', 30, 3.7, '007', 70);
insert into skill values ('005', 'Bless', '005', 13, 0.8, '010', 15);
insert into skill values ('006', 'Spore Charm', '004', 22, 1.3, '002', 22);

select * from skill


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

select * from weaponType


delete from weapon;

insert into weapon values ('001', 'Megasonic Thunder Rapier', '002', '009', 4.3);
insert into weapon values ('002', 'Twirling Wind Breaker', '004', '006', 2.7);
insert into weapon values ('003', 'Forbidden Elder Wand', '005', '007', 3.1);
insert into weapon values ('004', 'Throwable Tasty Cheeseball', '008', '001', 5.4);
insert into weapon values ('005', 'The Literally King''s Cross', '010', '003', 1.8);
insert into weapon values ('006', 'Wook Stick', '005', '006', 0.6);

select * from weapon


delete from users;

insert into users values ('001', 'GM1', 9999, NULL, NULL);
insert into users values ('002', 'GM2', 9999, NULL, NULL);
insert into users values ('003', 'kjunwoo23', 48, '004', '004');
insert into users values ('004', 'IWoN''tHeAlyOu', 35, '005', '005');
insert into users values ('005', 'iStartedyesterday', 7, '003', '006');

select * from users


delete from userSkills;

insert into userSkills values ('003', '004');
insert into userSkills values ('004', '005');
insert into userSkills values ('003', '006');

select * from userSkills


delete from GM;

insert into GM values ('001', '001', 'HeadGM');
insert into GM values ('002', '002', 'Staff');

select * from GM


delete from banList;

insert into banList values ('001', '004', '001', '2024-04-12 04:47:22', '2024-04-18 04:47:22');

select * from banList


delete from auction;

insert into auction values ('001', '002', '003', 15000);
insert into auction values ('002', '003', '005', 27000);

select * from auction


delete from boss;

insert into boss values ('001', 'Elsa and Olaf', '004', 500, 20, 5, 4);
insert into boss values ('002', 'Fenrir', '006', 5000, 200, 40, 8);

select * from boss


delete from raidPartyRoom;

insert into raidPartyRoom values ('001', 'anybody need olaf carrot?', '001', '005', 1);
insert into raidPartyRoom values ('002', 'wolf raid / fire dps welcome', '002', '003', 3);

select * from raidPartyRoom
