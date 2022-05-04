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
    mess = f'Привет!, <b>{message.from_user.first_name} <u>{message.from_user.last_name}</u></b>\n' \
           f'Ты можешь создать ссылку для текста и получить текст по ссылке\n' \
           f'// Работает только с текстом без спецсимволов и перевода строки('
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
            request = f'{host}/content/create/?content={message.text}'
            response = requests.get(request)
            mess = f'{host}/content/{response.text}'
            if (response.text == ""):
                mess = "Не удалось создать ссылку("
        elif (command == 2):
            print(message.text)
            response = requests.get(message.text)
            mess = response.text
    except Exception as e:
        mess = f'Ошибка: {str(e)}'
    bot.send_message(message.chat.id, mess)


bot.polling(none_stop=True)
