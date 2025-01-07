import requests
from datetime import datetime
import pandas as pd
import matplotlib.pyplot as plt
from io import BytesIO
from telegram import Update
from telegram.ext import ApplicationBuilder, MessageHandler, ContextTypes
from telegram.ext import filters
import re
import os
from dotenv import load_dotenv
import telebot
from telebot import types # для указание типов
import config

load_dotenv()
token = os.getenv('TOKEN_TG')  # Замените на токен вашего бота
bot = telebot.TeleBot(token)
howToBot = True
configGroup = False

def getInfo(group, date):
    today = date
    t = 0.1961507760667558
    action = 'show'
    
    # Формируем URL с параметрами
    url = 'https://www.usue.ru/schedule/'
    params = {
        't': t,
        'action': action,
        'startDate': today,
        'endDate': today,
        'group': group
    }

    # Отправляем GET-запрос
    response = requests.get(url, params=params)

    # Проверяем статус ответа
    if response.status_code == 200:
        print("Запрос выполнен успешно!")
        
        # Парсим JSON-ответ
        data = response.json()
        
        # Проверяем наличие данных в первом элементе
        if len(data) > 0 and 'pairs' in data[0]:
            pairs = data[0]['pairs']  # Получаем массив pairs
            
            # Список для хранения извлеченных данных
            extracted_data = []
            
            for pair in pairs:
                # Извлекаем необходимые данные
                record_n = pair.get('N', '')  # Запись N
                record_time = pair.get('time', '')  # Время
                
                # Извлекаем schedulePairs, если существует
                schedule_pairs = pair.get('schedulePairs', [])
                for schedule in schedule_pairs:
                    subject = schedule.get('subject', '')  # Предмет
                    teacher = schedule.get('teacher', '')  # Преподаватель
                    group = schedule.get('group', '')      # Группа
                    aud = schedule.get('aud', '')          # Аудитория
                    
                    # Добавляем данные в список
                    extracted_data.append([record_n, record_time, subject, teacher, group, aud])
            
            # Создаем DataFrame для удобства работы с данными и подписываем столбцы
            df = pd.DataFrame(extracted_data, columns=['Номер', 'Время', 'Предмет', 'Преподаватель', 'Группа', 'Аудитория'])
            
            # Выводим DataFrame или сохраняем его в файл
            print(df)
            return(df)
        else:
            print("Нет данных в первом элементе ответа.")
    else:
        print(f"Ошибка при выполнении запроса: {response.status_code}")
    
@bot.message_handler(commands=['start'])
def start(message):
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
    btn1 = types.KeyboardButton("Как пользоваться ботом")
    btn2 = types.KeyboardButton("Подписаться на рассылку расписания")
    markup.add(btn1, btn2)
    bot.send_message(message.chat.id, text="Привет, {0.first_name}".format(message.from_user), reply_markup=markup)
@bot.message_handler(content_types=['text'])
async def func(update: Update, context: ContextTypes.DEFAULT_TYPE):
    message = update.message  # Get the message from the update object
    if message.text == "Как пользоваться ботом":
        markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
        bot.send_message(message.chat.id, text="В ручном режиме: прислать название группы (н.п. 'ИНО ЗБ ПОАС-24'), при желании добавить дату (н.п. 'ИНО ЗБ ПОАС-24 18.12.2024')\nВ автоматическом режиме: нажать на кнопку 'Подписаться на рассылку расписания' и ввести название группы (н.п. 'ИНО ЗБ ПОАС-24'), при желании добавить время (н.п. 'ИНО ЗБ ПОАС-24 14:00') (по стандарту присылается в 8:30)")
        btne1 = types.KeyboardButton("Не показывать больше")
        back = types.KeyboardButton("Вернуться в главное меню")
        markup.add(btne1, back)
    elif message.text == "Не показывать больше":
        global howToBot
        howToBot = False
        await back_to_menu(update, context)
    elif message.text == "Подписаться на рассылку расписания":
        bot.send_message(message.chat.id, text="В ручном режиме: прислать название группы (н.п. 'ИНО ЗБ ПОАС-24'), при желании добавить дату (н.п. 'ИНО ЗБ ПОАС-24 18.12.2024')\nВ автоматическом режиме: нажать на кнопку 'Подписаться на рассылку расписания' и ввести название группы (н.п. 'ИНО ЗБ ПОАС-24'), при желании добавить время (н.п. 'ИНО ЗБ ПОАС-24 14:00') (по стандарту присылается в 8:30)")
        subs = types.KeyboardButton("Подтвердить подписку")
        back = types.KeyboardButton("Вернуться в главное меню")
        markup.add(btne1, back)
    elif message.text == "Вернуться в главное меню":
        await back_to_menu(update, context)
    elif configGroup!=True:
        await send_dataframe(update, context)
    elif configGroup==True:
        bot.send_message(message.chat.id, text="В ручном режиме: прислать название группы (н.п. 'ИНО ЗБ ПОАС-24'), при желании добавить дату (н.п. 'ИНО ЗБ ПОАС-24 18.12.2024')\nВ автоматическом режиме: нажать на кнопку 'Подписаться на рассылку расписания' и ввести название группы (н.п. 'ИНО ЗБ ПОАС-24'), при желании добавить время (н.п. 'ИНО ЗБ ПОАС-24 14:00') (по стандарту присылается в 8:30)")

async def back_to_menu(update: Update, context: ContextTypes.DEFAULT_TYPE):
    message = update.message
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
    btn2 = types.KeyboardButton("Подписаться на рассылку расписания")
    if howToBot:
        btn1 = types.KeyboardButton("Как пользоваться ботом")
        markup.add(btn1, btn2)
    else:
        markup.add(btn2)
        bot.send_message(message.chat.id, text="Вы вернулись в главное меню", reply_markup=markup)
async def send_dataframe(update: Update, context: ContextTypes.DEFAULT_TYPE):
    message_text = update.message.text.strip()  # Получаем текст сообщения
    
    # Регулярное выражение для поиска даты в формате dd.mm.yyyy
    date_pattern = r'(\d{2}\.\d{2}\.\d{4})'
    
    match = re.search(date_pattern, message_text)  # Ищем дату в сообщении
    
    if match:
        date_str = match.group(1)  # Извлекаем найденную дату
        
        try:
            date = datetime.strptime(date_str, "%d.%m.%Y").strftime("%d.%m.%Y")
        except ValueError:
            await update.message.reply_text("Неверный формат даты. Используйте dd.mm.yyyy.")
            return
        
        group = message_text[:match.start()].strip()  # Получаем группу до даты и убираем пробелы
    else:
        date = datetime.now().strftime("%d.%m.%Y")  # Используем текущую дату по умолчанию
        group = message_text.strip()  # Весь текст - это группа
    
    await update.message.reply_text(f"Запрашиваю расписание для группы: {group}...")
    
    df = getInfo(group,date)  # Получаем DataFrame
    
    if df is not None and not df.empty:
        column_widths = [0.03, 0.15, 0.50, 0.20, 0.10, 0.7]
        fig, ax = plt.subplots(figsize=(17, 5))  # Размер изображения

        ax.axis('tight')
        ax.axis('off')

        table = ax.table(cellText=df.values,
                         colLabels=df.columns,
                         cellLoc='center',
                         loc='center')

        # Установка ширины столбцов
        for i, width in enumerate(column_widths):
            table.auto_set_column_width(i)
            # Увеличиваем высоту ячеек
            table.scale(width, 1.5)  # Измените второй аргумент для высоты по необходимости

        table.auto_set_font_size(False)
        table.set_fontsize(20)  # Установка размера шрифта

        # Сохранение изображения в буфер
        buf = BytesIO()
        plt.savefig(buf, format='png', bbox_inches='tight', pad_inches=0.1)  # Используем bbox_inches для обрезки
        buf.seek(0)  # Перемещаем указатель в начало буфера

        # Отправляем изображение пользователю
        await context.bot.send_photo(chat_id=update.effective_chat.id, photo=buf)
        
        plt.close(fig)  # Закрываем фигуру после отправки

    else:
        await update.message.reply_text("Не удалось получить расписание. Проверьте правильность написания группы или дня недели.")
    pass

def main():
    print('Запуск')
    application = ApplicationBuilder().token(token).build()
    application.add_handler(MessageHandler(filters.TEXT & ~filters.COMMAND, func))
    application.run_polling()

if __name__ == '__main__':
    main()