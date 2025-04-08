# Задача 1
# API для простого управления задачами пользователя(Task Management Service)

### Стек
* ASP .NET 8
* RabbitMq
* Masstransit
* PostgreSQL
* MongoDB
* Quartz
* XUnit

Запускается через docker-compose, там же разворачивются все бд, и кролик.

Основная API на порту 5001
Стандартные круды, позволяющие:
- Добавить новую таску
- Изменить статус таски
- Достать все таски
- Удалить таску
- Обновить тайтл и описание таски

Все взаимодействие выполняется асинхронно, отправляя сообщение в кролик и затем консюмеры их считывают, на все процессы затрагивающие изменение таски отправляется сообщение на логирование во второй сервис на порту 5002 (Там единственный метод, который достает все залогированные данные)

Добавлено информационное логирование запросов, время выполнения, ошибки логируются в файлик.

Логика удаления тасок сделана так -> поступает запрос на удаление, таска помечается в бд на удаление, Quartz job`а выполняет очищение помеченных тасок в определенном интервале (для удобства сделал интерфал в несколько секунд)

# Задача 2
create or replace function GetDailyPayments(client_id bigint,start_date date,end_date date)
returns table(PaymentDate DATE,TotalAmount MONEY)
    language sql
as $$
    select *
    from (select gen_days::date as Dt, coalesce(sum(cp.Amount), 0::money) as Сумма
    from generate_series(start_date, end_date, interval '1 day') gen_days
    left join ClientPayments cp
        on cp.Dt::date = gen_days::date and cp.ClientId = client_id
    group by gen_days
    order by gen_days) as DС
    $$;

Сложность была только в том, чтобы понять как получить список всех дат в определенном промежутке, нагуглил generate_series :)
