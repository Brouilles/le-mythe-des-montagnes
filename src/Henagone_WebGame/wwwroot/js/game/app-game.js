/// <reference path="jquery.d.ts"/>
// Global Variables //
var g_lang = "";
var g_currentJobId = 0;
var g_currentCompanionId = 0;
var g_currentCompanionSendBackId = 0;
var g_currentCompanionQuestId = 0;
var g_skillPointsDiv = 0;
//-> Battle
var g_currentSkill = 1;
var g_currentTimer = 6;
///////////////////////
var g_areaArray = [
    "_ShopPartial.cshtml",
    "_TavernPartial.cshtml",
    "_ArenaPartial.cshtml",
    "_HeroQuestPartial.cshtml",
    "_AtWorkPartial.cshtml",
    "_CompanionManagementPartial.cshtml",
    "_ManorPartial.cshtml",
    "_AtNarrativeQuestPartial.cshtml"
];
// Methods
var NotificationType;
(function (NotificationType) {
    NotificationType[NotificationType["Success"] = 0] = "Success";
    NotificationType[NotificationType["Information"] = 1] = "Information";
    NotificationType[NotificationType["Alert"] = 2] = "Alert";
})(NotificationType || (NotificationType = {}));
var UI;
(function (UI) {
    UI[UI["City"] = 0] = "City";
    UI[UI["Shop"] = 1] = "Shop";
    UI[UI["Tavern"] = 2] = "Tavern";
    UI[UI["Arena"] = 3] = "Arena";
    UI[UI["HeroQuest"] = 4] = "HeroQuest";
    UI[UI["AtWork"] = 5] = "AtWork";
    UI[UI["CompanionManagement"] = 6] = "CompanionManagement";
    UI[UI["Manor"] = 7] = "Manor";
    UI[UI["AtNarrativeQuest"] = 8] = "AtNarrativeQuest";
})(UI || (UI = {}));
class GameClass {
    constructor() {
        this.UI = UI.City;
        this.player = new PlayerClass();
    }
    // Methods
    Update() {
        $('#JsGold').text(this.gold);
        $('#JsArtifacts').text(this.artifacts);
        this.player.Update();
    }
    GetPlayer() {
        return this.player;
    }
}
class PlayerClass {
    // Methods
    Update() {
        $('.JsLevel').text(this.Level);
        $('#JsLife').css('width', this.Life + '%');
        $('#JsLifeText').text(this.Life);
        $('#JsXp').css('width', this.Xp + '%');
        $('#JsXpText').text(this.Xp);
        $('#JsStrength').text(this.Strength);
        $('#JsDefense').text(this.Defense);
        $('#JsEnergy').text(this.Energy);
        $('#skillPointsDiv').text(g_skillPointsDiv);
        if (this.StrengthAdd != 0 || this.StrengthAdd != null)
            $('#JsStrengthAdd').text("(+" + this.StrengthAdd + ")");
        if (this.DefenseAdd != 0 || this.DefenseAdd != null)
            $('#JsDefenseAdd').text("(+" + this.DefenseAdd + ")");
        if (this.EnergyAdd != 0 || this.EnergyAdd != null)
            $('#JsEnergyAdd').text("(+" + this.EnergyAdd + ")");
    }
}
////////////////////////////////////////////////////////
// Link between ASP.NET Core and TypeScript (JavaScript)
////////////////////////////////////////////////////////
var game = new GameClass();
function GameInit(lang, ShopString, JobsString, ArenaString, TavernString, ApothecaryString, ManorString) {
    g_lang = lang;
    GameUpdateCharacter();
    GameUpdateGold();
    GameUpdateArtifacts();
    GameUpdateSkills();
    GameUpdateInventoryWeight();
    GameUpdateInventoryEquipment();
    GameUpdateRankingHunt();
    // Update UI
    $('#layer1').tooltip({ title: ShopString, container: 'body' }); // Shop
    $('#layer1').click(function () { LoadArea(UI.Shop); });
    $('#g6019Mobile').click(function () { LoadArea(UI.Shop, null, 1); });
    $('#layer5').tooltip({ title: JobsString, container: 'body' }); // Jobs
    $('#layer5, #g12664Mobile').click(function () {
        $('#md-jobs').niftyModal();
        $('input:radio[name="radJob"]').change(function () {
            g_currentJobId = $(this).val();
            $('#jobsProceed').prop("disabled", false);
        });
    });
    $('#layer2').tooltip({ title: ArenaString, container: 'body' }); // Arena
    //$('#layer1-8').click(function () { LoadArea(UI.Arena); });
    $('#layer3').tooltip({ title: TavernString, container: 'body', placement: 'right' }); // Tavern
    $('#layer3, #g5832Mobile').click(function () { LoadArea(UI.Tavern, null, 1); });
    $('#layer4').tooltip({ title: ApothecaryString, container: 'body' }); // Apothecary
    $('#layer4, #layer1-89Mobile').click(function () {
        $.getJSON("/Game/AjaxGetCarePrice", function (result) {
            $.each(result, function (key, val) { $('#apothecaryCarePrice').text(val); });
        });
        $('#md-apothecary').niftyModal();
    });
    $('#layer7').tooltip({ title: ManorString, container: 'body', placement: 'right' }); // Manor
    $('#layer7').click(function () { LoadArea(UI.Manor); });
    $('#layer1-6Mobile').click(function () { LoadArea(UI.Manor, null, 1); });
    // Companions
    $('#CompanionManagementOpen').click(function () { LoadArea(UI.CompanionManagement); });
    GameUpdateCompanion();
    // Update every 30 seconds
    (function () {
        //Verify if he is in Work (Job)
        $.ajax({
            dataType: "json",
            url: "/Game/AjaxGetWork",
            success: function (data) {
                var jobStart;
                $.each(data, function (key, val) {
                    if (val.Success == "true") {
                        g_currentJobId = val.JobId;
                        LoadArea(UI.AtWork, function () {
                            var tempAtWorkDate = val.AtWorkStart.split("/");
                            $('#atWorkProgressBar').progressTimer({
                                timeLimit: $('#TimeInput' + g_currentJobId).val(),
                                start: new Date(val.AtWorkStart),
                                baseStyle: 'progress-bar-primary progress-bar-striped active',
                                warningStyle: 'progress-bar-primary progress-bar-striped active',
                                completeStyle: 'progress-bar-success progress-bar-striped active',
                                eachIteration: function (elapsed) {
                                    var price = Math.ceil(((($('#TimeInput' + g_currentJobId).val() * 1000) - elapsed) / 900000) * 3);
                                    if (price < 1)
                                        $('#atWorkArtifactPrice').text('1');
                                    else
                                        $('#atWorkArtifactPrice').text(price);
                                },
                                onFinish: function () {
                                    $('#atWorkBtn').empty();
                                    $('#atWorkBtn').html('<a onclick="RecoverJob()" class="btn btn-space btn-success"><i class="icon icon-left mdi mdi-check"></i> ' + val.ButtonTxt + '</a>');
                                }
                            });
                        });
                    }
                });
            }
        });
        //Verify if he is in Quest (Hero Quest)
        $.ajax({
            dataType: "json",
            url: "/Game/AjaxGetHeroQuest",
            success: function (data) {
                $.each(data, function (key, val) {
                    if (val.Success == "true") {
                        StartHeroQuest(val.QuestId);
                    }
                });
            }
        });
        GameUpdateCharacter();
        setTimeout(arguments.callee, 30000);
    })();
}
function GameUpdateCharacter() {
    $.getJSON("/Game/AjaxGetCharacter", function (result) {
        $.each(result, function (key, val) {
            game.GetPlayer().Level = val.Level;
            game.GetPlayer().Xp = val.Xp;
            GameUpdateLife();
            game.GetPlayer().Defense = val.Defense;
            game.GetPlayer().Energy = val.Energy;
            game.GetPlayer().Strength = val.Strength;
            game.GetPlayer().DefenseAdd = val.DefenseAdd;
            game.GetPlayer().EnergyAdd = val.EnergyAdd;
            game.GetPlayer().StrengthAdd = val.StrengthAdd;
            g_skillPointsDiv = val.Points;
            if (g_skillPointsDiv > 0) {
                $('.awardSkillsPoints').show();
            }
            else {
                g_skillPointsDiv = 0;
                $('.awardSkillsPoints').hide();
            }
        });
        game.Update();
    });
}
function GameUpdateGold() {
    $.getJSON("/Game/AjaxGetGold", function (result) {
        $.each(result, function (key, val) {
            game.gold = val;
            game.Update();
        });
    });
}
function GameUpdateLife() {
    $.getJSON("/Game/AjaxGetLife", function (result) {
        $.each(result, function (key, val) {
            game.GetPlayer().Life = val;
            game.GetPlayer().Update();
        });
    });
}
function GameUpdateArtifacts() {
    $.getJSON("/Game/AjaxGetArtifacts", function (result) {
        $.each(result, function (key, val) {
            game.artifacts = val;
            game.Update();
        });
    });
}
function GameUpdateInventoryWeight() {
    $.getJSON("/Game/AjaxGetInventoryWeight", function (result) {
        $.each(result, function (key, val) {
            $('#JsWeight').text(val.Weight + "/" + val.Space);
        });
    });
}
// UI
function LoadArea(ui, callFunction, type = 0, parameter) {
    if (game.UI != UI.HeroQuest) {
        if (game.UI != UI.AtNarrativeQuest) {
            if (game.UI != UI.AtWork) {
                if (game.UI != UI.City) {
                    CloseArea();
                }
                // type: 0 -> Normal / 1 -> Resize
                $('#gameContentDiv').empty();
                $('#gamePanelDiv').css('background', '#fff');
                $('#gameContentDiv').show();
                $('#gameContentDiv').addClass('be-loading-active');
                $('#gameContentDiv').append('<div class="be-spinner"><svg width="40px" height="40px" viewBox="0 0 66 66" xmlns="http://www.w3.org/2000/svg"><circle fill="none" stroke-width="4" stroke-linecap="round" cx="33" cy="33" r="30" class="circle"></circle></svg></div>');
                game.UI = ui;
                var request = '/Game/GetModule?partialName=' + g_areaArray[ui.valueOf() - 1];
                if (parameter != null) {
                    request += '&param1=' + parameter;
                }
                $('#gameContentDiv').load(request, function () {
                    if (callFunction != null) {
                        callFunction();
                    }
                    if (type == 0 && $(window).width() >= 1200) {
                        $('#gameContentCityDiv').css('visibility', 'hidden');
                    }
                    else {
                        $('#gameContentCityDiv').css('display', 'none');
                        $('#gameContentDiv').css('position', 'initial');
                    }
                    $('#gameContentDiv').removeClass('be-loading-active');
                });
            }
        }
    }
}
function CloseArea() {
    game.UI = UI.City;
    $('#gameContentDiv').empty();
    $('#gameContentDiv').hide();
    $('#gamePanelDiv').css('background', 'none');
    $('#gameContentCityDiv').css('display', 'block');
    $('#gameContentDiv').css('position', 'absolute');
    $('#gameContentCityDiv').css('visibility', 'visible');
}
function CloseAreaAccept() {
    $('#md-closeAreaAccept').niftyModal();
}
function LoadIndexArea(partialName, divName, append, callFunction, parameter) {
    $(divName).empty();
    var request = '/Game/GetModuleIndex?partialName=' + partialName;
    if (parameter != null) {
        request += '&param1=' + parameter;
    }
    if (append == false || append == null) {
        $(divName).load(request, function () {
            if (callFunction != null) {
                callFunction();
            }
        });
    }
    else {
        $.get(request, function (data) { $(divName).after(data); if (callFunction != null) {
            callFunction();
        } });
    }
}
function NotificationAlert(title, msg, type = NotificationType.Information, native = false) {
    var notificationTypeColor = "primary";
    if (type == NotificationType.Success) {
        notificationTypeColor = "success";
    }
    else if (type == NotificationType.Alert) {
        notificationTypeColor = "danger";
    }
    $.gritter.add({
        title: title,
        text: msg,
        class_name: 'color ' + notificationTypeColor
    });
    if (native) {
        if ('Notification' in window) {
            Notification.requestPermission(function (permission) {
                var notification = new Notification(title, { body: msg });
            });
        }
    }
}
function UICenterDiv(divName) {
    if ($(window).width() >= 1200) {
        $(divName).css({
            'position': 'absolute',
            'left': '50%',
            'top': '50%',
            'margin-left': -$(divName).outerWidth() / 2,
            'margin-top': -$(divName).outerHeight() / 2
        });
    }
    else {
        $(divName).css({
            'position': 'inherit',
            'left': 'initial',
            'top': 'initial',
            'margin-left': 'initial',
            'margin-top': 'initial'
        });
    }
}
// Character
function SkillIncrease(StatId) {
    $.ajax({
        data: { id: StatId },
        url: '/Game/AjaxAddSkill',
        success: function (data) {
            GameUpdateCharacter();
        }
    });
}
// Shop
function LoadSaleTab() {
    $('#sale').load("/Game/GetModule?partialName=_ShopSalePartial.cshtml");
}
function ItemDescription(itemId) {
    var div = $('#itemPopup' + itemId);
    if (div.css('display') == 'none') {
        div.css('display', 'block');
    }
    else {
        div.css('display', 'none');
    }
}
function PurchaseItem(itemId) {
    if (game.UI == UI.Shop) {
        $.ajax({
            data: { id: itemId },
            url: '/Game/AjaxPurchaseItem',
            success: function (data) {
                $.each(data, function (key, val) {
                    if (val.Success == "true") {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    }
                    else {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                    }
                    GameUpdateGold();
                    GameUpdateInventoryWeight();
                    GameUpdateInventoryBricABrac();
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
            }
        });
    }
}
function PurchaseItemArtifact(itemId) {
    if (game.UI == UI.Shop) {
        $.ajax({
            data: { id: itemId },
            url: '/Game/AjaxPurchaseItemArtifact',
            success: function (data) {
                $.each(data, function (key, val) {
                    if (val.Success == "true") {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    }
                    else {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                    }
                    GameUpdateArtifacts();
                    GameUpdateInventoryWeight();
                    GameUpdateInventoryBricABrac();
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
            }
        });
    }
}
function SaleItemDescription(itemId) {
    var div = $('#saleItemPopup' + itemId);
    if (div.css('display') == 'none') {
        div.css('display', 'block');
    }
    else {
        div.css('display', 'none');
    }
}
function ConvertArtifactToGold() {
    if (game.UI == UI.Shop) {
        $.ajax({
            url: '/Game/AjaxConvertArtifactToGold',
            success: function (data) {
                $.each(data, function (key, val) {
                    if (val.Success == "true") {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                        GameUpdateGold();
                        GameUpdateArtifacts();
                    }
                    else {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                    }
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
            }
        });
    }
}
function SaleItem(itemId, id) {
    if (game.UI == UI.Shop) {
        $.ajax({
            data: { id: itemId },
            url: '/Game/AjaxSaleItem',
            success: function (data) {
                $.each(data, function (key, val) {
                    if (val.Success == "true") {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    }
                    else {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                    }
                    GameUpdateGold();
                    GameUpdateInventoryWeight();
                    GameUpdateInventoryBricABrac();
                    $('#SaleItem' + id).hide();
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
            }
        });
    }
}
// Job
function StartJob() {
    if (g_currentJobId != 0) {
        $.ajax({
            data: { id: g_currentJobId },
            url: '/Game/AjaxStartWork',
            success: function (data) {
                $.each(data, function (key, val) {
                    if (val.Success == "true") {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                        LoadArea(UI.AtWork, function () {
                            $('#atWorkProgressBar').progressTimer({
                                timeLimit: $('#TimeInput' + g_currentJobId).val(),
                                baseStyle: 'progress-bar-primary progress-bar-striped active',
                                warningStyle: 'progress-bar-primary progress-bar-striped active',
                                completeStyle: 'progress-bar-success progress-bar-striped active',
                                eachIteration: function (elapsed) {
                                    var price = Math.ceil(((($('#TimeInput' + g_currentJobId).val() * 1000) - elapsed) / 900000) * 3);
                                    if (price < 1)
                                        $('#atWorkArtifactPrice').text('1');
                                    else
                                        $('#atWorkArtifactPrice').text(price);
                                },
                                onFinish: function () {
                                    $('#atWorkBtn').empty();
                                    NotificationAlert(val.Title, val.FinishedMsg, NotificationType.Information, true);
                                    $('#atWorkBtn').html('<a onclick="RecoverJob()" class="btn btn-space btn-success"><i class="icon icon-left mdi mdi-check"></i> ' + val.ButtonTxt + '</a>');
                                }
                            });
                            var price = Math.ceil(($('#TimeInput' + g_currentJobId).val() / 900) * 3);
                            $('#atWorkArtifactPrice').text(price);
                        });
                    }
                    else {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                    }
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
            }
        });
    }
}
function RecoverJob() {
    $.ajax({
        url: '/Game/AjaxRecoverWork',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    GameUpdateGold();
                    CloseArea();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function AccelerateJob() {
    $.ajax({
        url: '/Game/AjaxAccelerateWork',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    $('#atWorkBtn').empty();
                    $('#atWorkBtn').html('<a onclick="RecoverJob()" class="btn btn-space btn-success"><i class="icon icon-left mdi mdi-check"></i> ' + val.ButtonTxt + '</a>');
                    GameUpdateArtifacts();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function StopJob() {
    $.ajax({
        url: '/Game/AjaxStopWork',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Information);
                    CloseArea();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
// Apothecary
function Care() {
    $.ajax({
        url: '/Game/AjaxCare',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    GameUpdateGold();
                    GameUpdateLife();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function NiftyResetCharacter() {
    $('#md-resetCharacter').niftyModal();
}
function ResetStats() {
    $.ajax({
        url: '/Game/AjaxResetStats',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    GameUpdateCharacter();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function PurchasePotion(potionId) {
    $.ajax({
        data: { id: potionId },
        url: '/Game/AjaxPurchasePotion',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    GameUpdateCharacter();
                    GameUpdateGold();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
// Tavern
function PurchaseCompanion(itemId) {
    $.ajax({
        data: { id: itemId },
        url: '/Game/AjaxPurchaseCompanion',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    GameUpdateGold();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
// Hero Quest
function StartHeroQuest(questId) {
    LoadArea(UI.HeroQuest, null, 0, questId);
}
function AccelerateHeroQuest(stepVal) {
    $.ajax({
        url: '/Game/AjaxAccelerateHeroQuest',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    GameUpdateArtifacts();
                    $('#heroQuestDivCenter').empty();
                    $('#heroQuestDivCenter').html('<a onclick="HeroQuestNextStep(2)" class="btn btn-space btn-primary">' + val.BtnNextStep + '</a>');
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function HeroQuestNextStep(step) {
    $.getJSON("/Game/AjaxGetNextStep?id=" + step, function (result) {
        $.each(result, function (key, val) {
            if (val.Step == 1) {
                $('#heroQuestDivCenter').progressTimer({
                    timeLimit: val.Time,
                    start: new Date(Date.parse(val.Start)),
                    baseStyle: 'progress-bar-primary progress-bar-striped active',
                    warningStyle: 'progress-bar-primary progress-bar-striped active',
                    completeStyle: 'progress-bar-success progress-bar-striped active',
                    eachIteration: function (elapsed) {
                        var price = Math.ceil((((val.Time * 1000) - elapsed) / 900000) * 3);
                        if (price < 1) {
                            $('#atHeroQuestArtifactPrice').text('1');
                        }
                        else {
                            $('#atHeroQuestArtifactPrice').text(price);
                        }
                    },
                    onFinish: function () {
                        $('#heroQuestDivCenter').empty();
                        $('#heroQuestDivCenter').html('<a onclick="HeroQuestNextStep(2)" class="btn btn-space btn-primary">' + val.BtnNextStep + '</a>');
                        NotificationAlert('Le mythe des montagnes', val.BtnNextStep, NotificationType.Information, true);
                    }
                });
                $('#heroQuestDivCenter').append('<a onclick="AccelerateHeroQuest()" class="btn btn-space btn-success"><i class="icon icon-left mdi mdi-check"></i> ' + val.BtnFinished + ' (<span id="atHeroQuestArtifactPrice">-</span> ' + val.ArtifactTxt + ')</a>');
            }
            else if (val.Step == 2) {
                $('#heroQuestDivCenter').empty();
                $('#heroQuestDivCenter').load("/Game/GetModule?partialName=_AtBattlePVEPartial.cshtml", function () {
                    $.getJSON("/Game/AjaxRealTimeBattle", function (result) {
                        $.each(result, function (key, val) {
                            if (val.finished != "true") {
                                UICenterDiv('#heroQuestDivCenter');
                                // User
                                $('#BattleUserLifeDiv').css('width', val.UserLifePercentage + '%');
                                $('#BattleUserLifeText').text(val.UserLife);
                                $('#BattleUserEnergyDiv').css('width', val.UserEnergyPercentage + '%');
                                $('#BattleUserEnergyText').text(val.UserEnergy);
                                // Monster
                                $('#BattleMonsterLifeDiv').css('width', val.EntityLifePercentage + '%');
                                $('#BattleMonsterLifeText').text(val.EntityLife);
                                $('#BattleMonsterEnergyDiv').css('width', val.EntityEnergyPercentage + '%');
                                $('#BattleMonsterEnergyText').text(val.EntityEnergy);
                                // Timer
                                $('#BattleTimer').progressTimer({
                                    timeLimit: g_currentTimer,
                                    baseStyle: 'progress-bar-primary',
                                    warningStyle: 'progress-bar-primary',
                                    completeStyle: 'progress-bar-primary',
                                    allTheTime: function (settings) {
                                        if (g_currentTimer != settings.timeLimit) {
                                            settings.timeLimit = g_currentTimer;
                                        }
                                    },
                                    onFinish: function () {
                                        BattleUpdateProgressTimer('#BattleTimer', new Date(), 'primary', 'HeroQuestNextStep(3)');
                                        AtBattlePVE('HeroQuestNextStep(3)');
                                    }
                                });
                            }
                            else {
                                GameUpdateRankingHunt();
                                $('#heroQuestDivCenter').empty();
                                $('#heroQuestDivCenter').html('<a onclick="HeroQuestNextStep(3)" class="btn btn-space btn-primary">' + val.BtnNextStep + '</a>');
                            }
                        });
                    });
                });
            }
            else if (val.Step == 3) {
                GameUpdateRankingHunt();
                GameUpdateInventoryWeight();
                GameUpdateInventoryBricABrac();
                GameUpdateCharacter();
                var txtString = '<div class="row row-centered"><div class="col-md-4 col-centered">';
                var temporaryTable = '';
                for (var i = 0; i < val.ItemNumber; i++) {
                    temporaryTable += '<tr><td class="user-avatar">';
                    if (val.ItemArray[i].Img != "none") {
                        temporaryTable += '<img alt="Avatar" src="' + val.ItemArray[i].Img + '">';
                    }
                    temporaryTable += val.ItemArray[i].Title + '</td><td>' + val.ItemArray[i].Text + '</td></tr>';
                }
                txtString += '<table class="table"><tbody><tr id="goldandartifacts"><td class="user-avatar"> <img alt="Gold" src= "/images/Game/gold.svg" > ' + val.Gold + " " + val.GoldTxt + '</td><td class="user-avatar" > <img style="transform: scale(1.5);" alt="Xp" src= "/images/Game/xp.svg"> ' + val.Xp + " " + val.XpTxt + '</td ></tr>' + temporaryTable + '</tbody></table>';
                txtString += '<div style="text-align: center;"><a onclick="HeroQuestNextStep(4)" class="btn btn-space btn-primary">' + val.BtnGetReward + '</a></div>';
                txtString += '</div></div>';
                $('#heroQuestDivCenter').empty();
                $('#heroQuestDivCenter').html(txtString);
                UICenterDiv('#heroQuestDivCenter');
            }
            else if (val.Step == 0 || val.Step == -1) {
                GameUpdateGold();
                GameUpdateLife();
                GameUpdateCharacter();
                CloseArea();
            }
        });
        game.GetPlayer().Update();
    });
}
// Hero Quest
function StartNarrativeQuest(questId) {
    LoadArea(UI.AtNarrativeQuest, null, 1, questId);
}
function AccelerateNarrativeQuest() {
    $.ajax({
        url: '/Game/AjaxAccelerateNarrativeQuest',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    GameUpdateArtifacts();
                    $('#heroQuestDivCenter').empty();
                    $('#heroQuestDivCenter').html('<a onclick="NarrativeQuestNextStep()" class="btn btn-space btn-primary">' + val.BtnNextStep + '</a>');
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function NarrativeQuestNextStep(btnEvent = null) {
    var request = "/Game/AjaxNarrativeGetNextStep";
    if (btnEvent != null) {
        request += "?id=" + btnEvent;
    }
    $.getJSON(request, function (result) {
        $.each(result, function (key, val) {
            if (val.Step == 0) {
                $('#heroQuestDivCenter').progressTimer({
                    timeLimit: val.Time,
                    start: new Date(val.Start),
                    baseStyle: 'progress-bar-primary progress-bar-striped active',
                    warningStyle: 'progress-bar-primary progress-bar-striped active',
                    completeStyle: 'progress-bar-success progress-bar-striped active',
                    eachIteration: function (elapsed) {
                        var price = Math.ceil((((val.Time * 1000) - elapsed) / 900000) * 3);
                        if (price < 1) {
                            $('#atNarrativeQuestArtifactPrice').text('1');
                        }
                        else {
                            $('#atNarrativeQuestArtifactPrice').text(price);
                        }
                    },
                    onFinish: function () {
                        $('#heroQuestDivCenter').empty();
                        NarrativeQuestNextStep();
                    }
                });
                $('#heroQuestDivCenter').append('<a onclick="AccelerateNarrativeQuest()" class="btn btn-space btn-success"><i class="icon icon-left mdi mdi-check"></i> ' + val.BtnFinished + ' (<span id="atNarrativeQuestArtifactPrice">-</span> ' + val.ArtifactTxt + ')</a>');
            }
            else if (val.Step == -10) {
                $("#questImgBackground").css('visibility', 'hidden');
                GameUpdateRankingHunt();
                GameUpdateInventoryWeight();
                GameUpdateInventoryBricABrac();
                GameUpdateCharacter();
                var txtString = '<div class="row row-centered"><div class="col-md-4 col-centered">';
                var temporaryTable = '';
                for (var i = 0; i < val.ItemNumber; i++) {
                    temporaryTable += '<tr><td class="user-avatar">';
                    if (val.ItemArray[i].Img != "none") {
                        temporaryTable += '<img alt="Avatar" src="' + val.ItemArray[i].Img + '">';
                    }
                    temporaryTable += val.ItemArray[i].Title + '</td><td>' + val.ItemArray[i].Text + '</td></tr>';
                }
                txtString += '<table class="table"><tbody><tr id="goldandartifacts"><td class="user-avatar"> <img alt="Gold" src= "/images/Game/gold.svg" > ' + val.Gold + " " + val.GoldTxt + '</td><td class="user-avatar" > <img style="transform: scale(1.5);" alt="Xp" src= "/images/Game/xp.svg"> ' + val.Xp + " " + val.XpTxt + '</td ></tr>' + temporaryTable + '</tbody></table>';
                txtString += '<div style="text-align: center;"><a onclick="NarrativeQuestNextStep()" class="btn btn-space btn-primary">' + val.BtnGetReward + '</a></div>';
                txtString += '</div></div>';
                $('#heroQuestDivCenter').empty();
                $('#heroQuestDivCenter').html(txtString);
                UICenterDiv('#heroQuestDivCenter');
            }
            else if (val.Step == -11 || val.Step == -12) {
                GameUpdateGold();
                GameUpdateLife();
                GameUpdateCharacter();
                CloseArea();
            }
            else {
                $("#questImgBackground").css('visibility', 'visible');
                if (val.Type == 1) {
                    $("#questImgBackground").css('visibility', 'visible');
                    $('#heroQuestDivCenter').empty();
                    $('#heroQuestDivCenter').load("/Game/GetModule?partialName=_AtNarrativePartial.cshtml");
                }
                else if (val.Type == 2) {
                    $("#questImgBackground").css('visibility', 'hidden');
                    $('#heroQuestDivCenter').empty();
                    $('#heroQuestDivCenter').load("/Game/GetModule?partialName=_AtBattlePVEPartial.cshtml", function () {
                        $.getJSON("/Game/AjaxRealTimeBattle?id=total", function (result) {
                            $.each(result, function (key, val) {
                                if (val.finished != "true") {
                                    UICenterDiv('#heroQuestDivCenter');
                                    // User
                                    $('#BattleUserLifeDiv').css('width', val.UserLifePercentage + '%');
                                    $('#BattleUserLifeText').text(val.UserLife);
                                    $('#BattleUserEnergyDiv').css('width', val.UserEnergyPercentage + '%');
                                    $('#BattleUserEnergyText').text(val.UserEnergy);
                                    // Monster
                                    $('#BattleMonsterLifeDiv').css('width', val.EntityLifePercentage + '%');
                                    $('#BattleMonsterLifeText').text(val.EntityLife);
                                    $('#BattleMonsterEnergyDiv').css('width', val.EntityEnergyPercentage + '%');
                                    $('#BattleMonsterEnergyText').text(val.EntityEnergy);
                                    // Timer
                                    $('#BattleTimer').progressTimer({
                                        timeLimit: g_currentTimer,
                                        baseStyle: 'progress-bar-primary',
                                        warningStyle: 'progress-bar-primary',
                                        completeStyle: 'progress-bar-primary',
                                        allTheTime: function (settings) {
                                            if (g_currentTimer != settings.timeLimit) {
                                                settings.timeLimit = g_currentTimer;
                                            }
                                        },
                                        onFinish: function () {
                                            BattleUpdateProgressTimer('#BattleTimer', new Date(), 'primary', 'NarrativeQuestNextStep()');
                                            AtBattlePVE('NarrativeQuestNextStep()');
                                        }
                                    });
                                }
                                else {
                                    $('#heroQuestDivCenter').empty();
                                    $('#heroQuestDivCenter').html('<a onclick="NarrativeQuestNextStep()" class="btn btn-space btn-primary">' + val.BtnNextStep + '</a>');
                                }
                            });
                        });
                    });
                }
            }
        });
        game.GetPlayer().Update();
    });
}
// Battle PVE
function AtBattlePVE(callFunction) {
    $.getJSON("/Game/AjaxRealTimeBattle?userSkill=" + g_currentSkill, function (result) {
        $.each(result, function (key, val) {
            if (val.finished != "true") {
                // Show card
                $('#atBattlePlayerSkill' + g_currentSkill).fadeIn("fast", function () {
                    $('#atBattlePlayerSkill' + g_currentSkill).delay(600).fadeOut(400);
                });
                $('#atBattleMonsterSkill' + val.EntitySkillId).fadeIn("fast", function () {
                    $('#atBattleMonsterSkill' + val.EntitySkillId).delay(600).fadeOut(400);
                });
                // General
                $('#lapsLeftDiv').text(parseInt($('#lapsLeftDiv').text()) - 1);
                var data = [
                    { label: "Rest", data: parseInt($('#lapsLeftDiv').text()) },
                    { label: "Pass", data: (20 - parseInt($('#lapsLeftDiv').text())) },
                ];
                $.plot('#laps-left', data, {
                    series: {
                        pie: {
                            radius: 0.75,
                            innerRadius: 0.58,
                            show: true,
                            highlight: { opacity: 0.1 },
                            label: { show: false }
                        }
                    },
                    grid: { hoverable: true },
                    legend: { show: false },
                    colors: ["#34a853;", "#f5f5f5"]
                });
                // User
                $('#BattleUserLifeDiv').css('width', val.UserLifePercentage + '%');
                $('#BattleUserLifeText').text(val.UserLife);
                $('#BattleUserEnergyDiv').css('width', val.UserEnergyPercentage + '%');
                $('#BattleUserEnergyText').text(val.UserEnergy);
                // Monster
                $('#BattleMonsterLifeDiv').css('width', val.EntityLifePercentage + '%');
                $('#BattleMonsterLifeText').text(val.EntityLife);
                $('#BattleMonsterEnergyDiv').css('width', val.EntityEnergyPercentage + '%');
                $('#BattleMonsterEnergyText').text(val.EntityEnergy);
            }
            else {
                GameUpdateRankingHunt();
                var txtString = "";
                if (val.Win == "false") {
                    txtString += "<h3>" + val.HeaderTitle + "</h3><br/>";
                }
                txtString += '<a onclick="' + callFunction + '" class="btn btn-space btn-primary" > ' + val.BtnNextStep + '</a>';
                $('#heroQuestDivCenter').empty();
                $('#heroQuestDivCenter').html(txtString);
                UICenterDiv('#heroQuestDivCenter');
            }
        });
    });
}
function BattleUpdateProgressTimer(div, userTime, barStyle, callFunction) {
    $(div).progressTimer({
        timeLimit: g_currentTimer,
        extra: 100,
        baseStyle: 'progress-bar-' + barStyle,
        warningStyle: 'progress-bar-' + barStyle,
        completeStyle: 'progress-bar-' + barStyle,
        allTheTime: function (settings) {
            if (g_currentTimer != settings.timeLimit) {
                settings.timeLimit = g_currentTimer;
            }
        },
        onFinish: function () {
            BattleUpdateProgressTimer(div, new Date(), barStyle, callFunction);
            AtBattlePVE(callFunction);
        }
    });
}
function AtBattlePVEBuff() {
    $.ajax({
        url: '/Game/AjaxRealTimeBattleBuff',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    $('#BattleUserEnergyDiv').css('width', '100%');
                    $('#BattleUserEnergyText').text('100%');
                    $('#UserStrengthAdd').text(parseInt($('#UserStrength').text()) + parseInt($('#UserStrengthAdd').text()));
                    $('#UserDefenseAdd').text(parseInt($('#UserDefense').text()) + parseInt($('#UserDefenseAdd').text()));
                    GameUpdateArtifacts();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function atBattleSelectSkill(skillId) {
    $('#skill' + g_currentSkill).removeClass('active');
    g_currentSkill = skillId;
    $('#skill' + g_currentSkill).addClass('active');
}
function atBattleSelectSpeed(speed) {
    $('#speedBtn' + g_currentTimer).removeAttr('class');
    $('#speedBtn' + g_currentTimer).addClass('btn btn-primary');
    g_currentTimer = speed;
    $('#speedBtn' + g_currentTimer).removeAttr('class');
    $('#speedBtn' + g_currentTimer).addClass('btn btn-success');
}
function atBattleAbandonConfirm() {
    $('#md-atBattleAbandon').niftyModal();
}
function atBattleAbandon() {
    $.ajax({
        url: '/Game/AjaxRealTimeBattleAbandon',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    $('#BattleUserLifeDiv').css('width', '0%');
                    $('#BattleUserLifeText').text('0');
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
// Companions 
function CompanionAddToFavorite(teamId) {
    $.ajax({
        data: { id: teamId },
        url: '/Game/AjaxAddFavoriteCompanion',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    $('#FavoriteBtn' + teamId).text(val.BtnText);
                    $('#FavoriteBtn' + teamId).attr("onclick", "CompanionDeleteToFavorite(" + teamId + ")");
                    GameUpdateCompanion();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function CompanionDeleteToFavorite(teamId) {
    $.ajax({
        data: { id: teamId },
        url: '/Game/AjaxDeleteFavoriteCompanion',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    $('#FavoriteBtn' + teamId).text(val.BtnText);
                    $('#FavoriteBtn' + teamId).attr("onclick", "CompanionAddToFavorite(" + teamId + ")");
                    GameUpdateCompanion();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function CompanionSendBackConfirm(teamId) {
    $('#md-deleteCompanion').remove();
    LoadIndexArea('_DeleteCompanionModalPartial.cshtml', '#md-apothecary', true, function () {
        $('#md-deleteCompanion').niftyModal();
    }, teamId);
}
function CompanionSendBack(teamId) {
    $.ajax({
        data: { id: teamId },
        url: '/Game/AjaxSendBackCompanion',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    $('#CompanionsItem' + teamId).hide();
                    GameUpdateCompanion();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function AddCompanionSlot() {
    $.ajax({
        url: '/Game/AjaxAddCompanionSlot',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
                GameUpdateArtifacts();
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function GameUpdateCompanion() {
    LoadIndexArea('_CompanionPartial.cshtml', '#companionDiv', false, function () {
        $.getJSON("/Game/AjaxGetOnQuest", function (result) {
            $.each(result, function (key, val) {
                if (key != "success") {
                    // Button
                    $('#onQuestBtn' + val.TeamId).empty();
                    $('#onQuestBtn' + val.TeamId).html('<button onclick="AccelerateCompanionQuestConfirm( ' + val.TeamId + ')" class="btn btn-default btn-success" type="button"><i class="icon mdi mdi-check" style="color: white;"></i></button>');
                    // Progress
                    $('#onQuestProgressBar' + val.TeamId).empty();
                    $('#onQuestProgressBar' + val.TeamId).progressTimer({
                        timeLimit: val.Time,
                        start: new Date(val.Start),
                        baseStyle: 'progress-bar-primary',
                        warningStyle: 'progress-bar-primary',
                        completeStyle: 'progress-bar-success',
                        onFinish: function () {
                            NotificationAlert(val.BtnText, "", NotificationType.Information, true);
                            $('#onQuestProgressBar' + val.TeamId).empty();
                            $('#onQuestBtn' + val.TeamId).empty();
                            $('#onQuestProgressBar' + val.TeamId).html('<a onclick="RecoverCompanionQuest(' + val.TeamId + ')" class="btn btn-space btn-success"><i class="icon icon-left mdi mdi-check"></i> ' + val.BtnText + '</a>');
                        }
                    });
                }
            });
        });
    });
}
function AccelerateCompanionQuestConfirm(teamId) {
    $('#md-accelerateCompanionQuest').remove();
    LoadIndexArea('_AccelerateCompanionModalPartial.cshtml', '#md-apothecary', true, function () {
        $('#md-accelerateCompanionQuest').niftyModal();
    }, teamId);
}
function AccelerateCompanionQuest(teamId) {
    $.ajax({
        data: { id: teamId },
        url: '/Game/AjaxAccelerateCompanionQuest',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    GameUpdateArtifacts();
                    GameUpdateCompanion();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function OpenCompanionQuestsWindow(teamId, classId) {
    $('#md-CompanionMission').remove();
    LoadIndexArea('_CompanionModalPartial.cshtml', '#md-apothecary', true, function () {
        g_currentCompanionId = teamId;
        $('#md-CompanionMission').niftyModal();
        $('input:radio[name="radCompanionQuest"]').change(function () {
            g_currentCompanionQuestId = $(this).val();
            $('#CompanionQuestProceed').prop("disabled", false);
        });
    }, classId);
}
function StartCompanionQuest() {
    if (g_currentCompanionQuestId != 0) {
        var currentCompanionId = g_currentCompanionId;
        $.ajax({
            data: { teamId: currentCompanionId, questId: g_currentCompanionQuestId },
            url: '/Game/AjaxCompanionStartQuest',
            success: function (result) {
                $.each(result, function (key, val) {
                    if (val.Success == "true") {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                        GameUpdateCompanion();
                    }
                    else {
                        NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                    }
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
            }
        });
    }
}
function RecoverCompanionQuest(teamId) {
    $.ajax({
        data: { id: teamId },
        url: '/Game/AjaxRecoverCompanionQuest',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    GameUpdateGold();
                    GameUpdateCharacter();
                    GameUpdateCompanion();
                    game.Update();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                    GameUpdateCompanion();
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
// Skils 
function GameUpdateSkills() {
    LoadIndexArea("_SkillsPartial.cshtml", "#skillsDiv");
}
function SelectSkill(skillId) {
    $.ajax({
        data: { id: skillId },
        url: '/Game/AjaxSelectSkill',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    if ($('#skillSelect' + skillId).hasClass('active')) {
                        $('#skillSelect' + skillId).removeClass('active');
                    }
                    else {
                        $('#skillSelect' + skillId).addClass('active');
                    }
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
// Inventory
function GameUpdateInventoryEquipment() {
    LoadIndexArea("_InventoryEquipmentPartial.cshtml", "#inventoryEquipmentDiv");
}
function GameUpdateInventoryBricABrac() {
    LoadIndexArea("_InventoryPartial.cshtml", "#inventoryBricabracDiv");
}
function EquipItem(itemId) {
    $.ajax({
        data: { id: itemId },
        url: '/Game/AjaxEquipItem',
        success: function (data) {
            $.each(data, function (key, val) {
                if (val.Success == "true") {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Success);
                    GameUpdateInventoryEquipment();
                    GameUpdateInventoryWeight();
                    GameUpdateInventoryBricABrac();
                    GameUpdateCharacter();
                }
                else {
                    NotificationAlert(val.Title, val.Msg, NotificationType.Alert);
                }
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
function UnequipItem(slotId) {
    $.ajax({
        data: { id: slotId },
        url: '/Game/AjaxUnequipItem',
        success: function () { GameUpdateInventoryEquipment(); GameUpdateInventoryWeight(); GameUpdateInventoryBricABrac(); GameUpdateCharacter(); },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            NotificationAlert("Error", "Internal Error: " + errorThrown, NotificationType.Alert);
        }
    });
}
// Ranking 
function GameUpdateRankingHunt() {
    LoadIndexArea("_HuntingPartial.cshtml", "#rankingHuntingDiv");
}
//# sourceMappingURL=app-game.js.map