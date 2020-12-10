# Tp_lab_2
# Лабораторная работа 2 
## Задание: 
Требуется разработать приложение или программный комплекс,
обменивающийся данными по сети в формате JSON, XML или
Protocol Buffers.
## Вариант 6
Сетевая игра «Стишок» для четырёх игроков. Каждый игрок по очереди пишет строчку общего стиха на основе строчки предыдущего игрока. Остальные строки он не видит. Игроки
проходят фиксированное количество итераций, и программа выводит общий стих, который в итоге получился.
## Реализация:
Проект Tp_server2 - серверная часть приложения, содержит класс ClientObject для представления объекта клиентов и привязки к каждому клиенту id. Создается отдельный поток для работы с каждым ClientObject, а также поток для прослушивания входящих соединений.
ServerObject - класс для серверной логики и управления подключениями.

Проект Tp_client2 - клиентская часть приложения.
Оба проекта содержат класс IncomingMessage для обмена объектами этого класса в Json формате. Для Json представления используется библиотека от NewtonSoft.
Взаимодейтсвие между проектами представлено в виде сокетов на TCP протоколе.
# Лабораторная работа 3
## Задание:
Добавьте в свой предыдущий проект возможность сохранения
состояния в виде периодического сохранения, либо в виде функций
импорта и экспорта. Выбранный формат для сериализации должен
иметь схему. В проекте обязателен код валидирующий данные.
Валидация должна производиться либо в программе при импорте
данных, либо в юнит-тестах, проверяющих корректность
сохранения состояния
## Реализация: 
Класс Data используется в качестве сохранения состояния программы. Функция SaveData сохраняет текущее состояние программы в файл в формате json, а именно username всех клиентов, количество итераций и текущий стих. Перед сохранением производится проверка json файла на корректность.
## Пример Json'a состояния программы:
{

    "clients": ["1", "2", "3", "4"],
    
    "iterations": 2,
    
    "poem": ["qwe", "123"]
}
## Json схема:
{

    "type": "object",
    
    "title": "The root schema",
    
    "description": "The root schema comprises the entire JSON document.",
    
    "default": {},
    
    "required": [
    
        "clients",
        
        "iterations",
        
        "poem"
        
    ],
    
    "properties": {
    
        "clients": {
        
            "type": "array",
            
            "title": "The clients schema",
            
            "description": "An explanation about the purpose of this instance.",
            
            "default": [],
            
            "additionalItems": false,
            
            "items": {
            
                "anyOf": [
                
                    {
                    
                        "type": "string",
                        
                        "title": "The first anyOf schema",
                        
                        "description": "An explanation about the purpose of this instance.",
                        
                        "default": "",
                        
                    }
                    
                ]
                
            }
            
        },
        
        "iterations": {
        
            "type": "integer",
            
            "title": "The iterations schema",
            
            "description": "An explanation about the purpose of this instance.",
            
            "default": 0,
            
        },
        
        "poem": {
        
            "type": "array",
            
            "title": "The poem schema",
            
            "description": "An explanation about the purpose of this instance.",
            
            "default": [],
            
            "additionalItems": false,
            
            "items": {
            
                "anyOf": [
                
                    {
                    
                        "type": "string",
                        
                        "title": "The first anyOf schema",
                        
                        "description": "An explanation about the purpose of this instance.",
                        
                        "default": "",
                        
                    }
                    
                ]
                
            }
            
        }
        
    },
    
    "additionalProperties": false
    
}
