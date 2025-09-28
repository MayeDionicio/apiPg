-- Verificar que tablas existen en la base de datos
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_type = 'BASE TABLE'
ORDER BY table_name;

-- Verificar específicamente la tabla Devocionales
SELECT EXISTS (
    SELECT FROM information_schema.tables 
    WHERE table_schema = 'public' 
    AND table_name = 'Devocionales'
) as devocionales_exists;

-- Si existe, mostrar su estructura
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_name = 'Devocionales' 
AND table_schema = 'public'
ORDER BY ordinal_position;