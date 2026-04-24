using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SLP.Core;

namespace SLP.Features.Names
{
    public class NamesModule : Module
    {
        public override string Name => "Names";
        public override Version Version => new(1, 0, 0);

        private readonly Random _random = new();
        private readonly Dictionary<Player, string> _names = new();
        private readonly Dictionary<Player, string> _customInfos = new();

        private Dictionary<RoleTypeId, string> ClassTitles { get; set; } = new()
        {
            {
                RoleTypeId.ClassD,
                "D-"
            },
            {
                RoleTypeId.Scientist,
                "Др. "
            },
            {
                RoleTypeId.FacilityGuard,
                "Офицер безопасности "
            },
            {
                RoleTypeId.NtfCaptain,
                "Капитан МОГ | Позывной: "
            },
            {
                RoleTypeId.NtfPrivate,
                "Рядовой МОГ | Позывной: "
            },
            {
                RoleTypeId.NtfSergeant,
                "Сержант МОГ | Позывной: "
            },
            {
                RoleTypeId.NtfSpecialist,
                "Специалист МОГ | Позывной: "
            },
            {
                RoleTypeId.ChaosConscript,
                "Новичок ПХ | Позывной: "
            },
            {
                RoleTypeId.ChaosMarauder,
                "Марадёр ПХ | Позывной: "
            },
            {
                RoleTypeId.ChaosRepressor,
                "Репрессор ПХ | Позывной: "
            },
            {
                RoleTypeId.ChaosRifleman,
                "Стрелок ПХ | Позывной: "
            }
        };

        private List<string> HumanNames { get; set; } =
        [
            "Александр",
            "Алексей",
            "Андрей",
            "Артём",
            "Борис",
            "Вадим",
            "Валентин",
            "Валерий",
            "Василий",
            "Виктор",
            "Владимир",
            "Вячеслав",
            "Владислав",
            "Геннадий",
            "Георгий",
            "Григорий",
            "Даниил",
            "Денис",
            "Дмитрий",
            "Евгений",
            "Егор",
            "Иван",
            "Игорь",
            "Илья",
            "Кирилл",
            "Константин",
            "Лев",
            "Леонид",
            "Максим",
            "Марк",
            "Матвей",
            "Михаил",
            "Никита",
            "Николай",
            "Олег",
            "Павел",
            "Пётр",
            "Роман",
            "Руслан",
            "Семён",
            "Сергей",
            "Станислав",
            "Степан",
            "Тимофей",
            "Фёдор",
            "Юрий",
            "Яков",
            "Ярослав"
        ];

        private List<string> HumanLastNames { get; set; } =
        [
            "Иванов",
            "Петров",
            "Сидоров",
            "Смирнов",
            "Кузнецов",
            "Попов",
            "Васильев",
            "Соколов",
            "Михайлов",
            "Новиков",
            "Фёдоров",
            "Морозов",
            "Волков",
            "Алексеев",
            "Лебедев",
            "Семёнов",
            "Егоров",
            "Павлов",
            "Козлов",
            "Степанов",
            "Волчков",
            "Суворов",
            "Бабушкин"
        ];

        private List<string> MilitaryNames { get; set; } =
        [
            "Орёл",
            "Вулкан",
            "Беркут",
            "Патриот",
            "Варяг",
            "Тайфун",
            "Волк",
            "Буран",
            "Гром",
            "Сокол",
            "Молот",
            "Альфа",
            "Барон",
            "Вепрь",
            "Гранит",
            "Дозор",
            "Егерь",
            "Жнец",
            "Зенит",
            "Икар",
            "Клинок",
            "Лорд",
            "Медведь",
            "Норд",
            "Охотник",
            "Пиран",
            "Рысь",
            "Скат",
            "Тигр",
            "Уголь",
            "Факел",
            "Хищник",
            "Цитадель",
            "Шквал",
            "Щит",
            "Эльбрус",
            "Юпитер",
            "Ястреб",
            "Амур",
            "Багира",
            "Бизон",
            "Борей",
            "Вихрь",
            "Ворон",
            "Гриф",
            "Дракон",
            "Змей",
            "Кобра",
            "Кондор",
            "Лев",
            "Лис",
            "Молот",
            "Опал",
            "Пантер",
            "Рок",
            "Рубин",
            "Сапсан",
            "Скорпион",
            "Сфинкс",
            "Топаз",
            "Ураган",
            "Феникс",
            "Халк",
            "Цепь",
            "Шериф",
            "Шторм",
            "Эфир",
            "Ягуар",
            "Агат",
            "Атлет",
            "Булат",
            "Валет",
            "Гектор",
            "Дозор",
            "Жук",
            "Зубр",
            "Коготь",
            "Колун",
            "Корсар",
            "Марс",
            "Минор",
            "Нептун",
            "Омар",
            "Пилот",
            "Рейнджер",
            "Русич",
            "Самум",
            "Сириус",
            "Стикс",
            "Страж",
            "Титан",
            "Тор",
            "Фобос",
            "Хирон",
            "Цербер",
            "Шах",
            "Шмель",
            "Эмир",
            "Юпитер",
            "Яр",
            "Аспид",
            "Вождь"
        ];

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            base.OnDisabled();
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            int age = _random.Next(25, 50);
            int dNumber = _random.Next(1, 999999999);

            string randomName =
                $"{HumanLastNames[_random.Next(HumanLastNames.Count)]} {HumanNames[_random.Next(HumanNames.Count)]}";
            string playerDisplayNickname = $"[{ev.Player.Id}] {randomName}";

            string militaryRandomName =
                $"{ClassTitles[ev.NewRole]} {MilitaryNames[_random.Next(MilitaryNames.Count)]}";

            if (ev.NewRole == RoleTypeId.Scp079)
            {
                _names[ev.Player] = ev.Player.DisplayNickname;
                _customInfos[ev.Player] = ev.Player.CustomInfo;
                return;
            }

            if (ev.Player.Role == RoleTypeId.Scp079)
            {
                ev.Player.DisplayNickname = _names[ev.Player];
                ev.Player.CustomInfo = _customInfos[ev.Player];
            }

            switch (ev.NewRole)
            {
                default:
                    ev.Player.DisplayNickname = playerDisplayNickname;
                    ev.Player.CustomInfo = $"Возраст: {age}";
                    break;

                case RoleTypeId.Scientist:
                    ev.Player.DisplayNickname = playerDisplayNickname;
                    ev.Player.CustomInfo = $"Возраст: {age} | УД 2 КЛАСС C";
                    ev.Player.ShowHint("The scientific method is the method not a suggestion box.", 2);
                    break;

                case RoleTypeId.FacilityGuard:
                    ev.Player.DisplayNickname = playerDisplayNickname;
                    ev.Player.CustomInfo = $"Возраст: {age} | УД 2 КЛАСС C";
                    ev.Player.ShowHint("Безопасность - главное", 2);
                    break;

                case RoleTypeId.ClassD:
                    ev.Player.DisplayNickname = $"[{ev.Player.Id}] D-{dNumber}";
                    ev.Player.CustomInfo = $"Возраст: {age} | Имя: {randomName}";
                    ev.Player.ShowHint("ЭЭЭЙ, ЧУВАК!!!", 2);
                    break;

                case RoleTypeId.Tutorial or RoleTypeId.Spectator or RoleTypeId.Overwatch:
                    ev.Player.DisplayNickname = null;
                    ev.Player.DisplayNickname = $"[{ev.Player.Id}] {ev.Player.Nickname}";
                    ev.Player.CustomInfo = null;

                    if (ev.NewRole == RoleTypeId.Tutorial)
                        ev.Player.ShowHint("ЗЛИЙ АДМИН ПРИДЁТ", 2);
                    if (ev.NewRole == RoleTypeId.Spectator)
                        ev.Player.ShowHint("ПОТРАЧЕНО", 2);
                    break;

                case RoleTypeId.None:
                    ev.Player.DisplayNickname = null;
                    ev.Player.CustomInfo = null;
                    break;

                case RoleTypeId.NtfPrivate or RoleTypeId.NtfSergeant or RoleTypeId.NtfSpecialist:
                    ev.Player.DisplayNickname = $"[{ev.Player.Id}] {militaryRandomName}";
                    ev.Player.CustomInfo = $"Возраст: {age} | УД 3 КЛАСС C | Имя: {randomName}";
                    ev.Player.ShowHint("Нельзя любить 939!", 2);
                    break;

                case RoleTypeId.ChaosConscript or RoleTypeId.ChaosMarauder or RoleTypeId.ChaosRepressor
                    or RoleTypeId.ChaosRifleman:
                    ev.Player.DisplayNickname = $"[{ev.Player.Id}] {militaryRandomName}";
                    ev.Player.CustomInfo = $"Возраст: {age} | Имя: {randomName}";
                    ev.Player.ShowHint("Было фонда, стало хаоса", 2);
                    break;

                case RoleTypeId.NtfCaptain:
                    ev.Player.DisplayNickname = $"[{ev.Player.Id}] {militaryRandomName}";
                    ev.Player.CustomInfo = $"Возраст: {age} | УД 4 КЛАСС B | Имя: {randomName}";
                    ev.Player.ShowHint("Капитан вроде, а вроде нет...", 2);
                    break;
            }

            if (ev.Player.IsScp)
            {
                ev.Player.DisplayNickname = ev.Player.Role.Name;
                ev.Player.CustomInfo = null;
            }
        }

        private void OnLeft(LeftEventArgs ev)
        {
            _names.Remove(ev.Player);
            _customInfos.Remove(ev.Player);
        }
    }
}