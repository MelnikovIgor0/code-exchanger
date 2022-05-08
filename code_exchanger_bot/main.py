import json
import telebot
import requests
from telebot import types

with open('bot.config') as f:
    lines = f.readlines()
    bot = telebot.TeleBot(lines[0][:-1])
    host = lines[1]
command = 0

@bot.message_handler(commands=['start', 'help'])
def start(message):
    mess = f'Привет!, <b>{message.from_user.first_name} {message.from_user.last_name}</b>\n' \
           f'Ты можешь создать ссылку для текста и получить текст по ссылке\n'
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True, row_width=2)
    content = types.KeyboardButton('/createlink')
    link = types.KeyboardButton('/getcontent')
    markup.add(content, link)
    bot.send_message(message.chat.id, mess, parse_mode='html', reply_markup=markup)


@bot.message_handler(commands=['createlink'])
def start(message):
    global command
    command = 1
    bot.send_message(message.chat.id, "Введите код")


@bot.message_handler(commands=['getcontent'])
def start(message):
    global command
    command = 2
    bot.send_message(message.chat.id, "Введите ссылку")


@bot.message_handler()
def get_user_text(message):
    global command
    mess = "Выберите действие"
    try:
        if (command == 1):
            request = f'{host}/content?content={message.text}'
            response = requests.post(request)
            mess = f'{host}/content?link={response.text}'
            if (response.text == ""):
                mess = "Не удалось создать ссылку("
        elif (command == 2):
            print(message.text)
            response = requests.get(message.text)
            mess = json.loads(response.text)["code"]
    except Exception as e:
        mess = f'Ошибка: {str(e)}'
    bot.send_message(message.chat.id, mess)


bot.polling(none_stop=True)