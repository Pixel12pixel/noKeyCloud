import {Button} from "@/shared/ui/button.tsx"
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "@/shared/ui/card.tsx"
import {
    Field,
    FieldDescription,
    FieldGroup,
    FieldLabel,
} from "@/shared/ui/field.tsx"
import {Input} from "@/shared/ui/input.tsx"
import {Form} from "react-router-dom";

interface RegisterFormProps extends React.ComponentProps<typeof Card> {
    error?: string;
    isSubmitting?: boolean;
}

export function RegisterForm({error, isSubmitting = false, ...props}: RegisterFormProps) {
    return (
        <Card {...props}>
            <CardHeader>
                <CardTitle>Create an account</CardTitle>
                <CardDescription>
                    Enter your information below to create your account
                </CardDescription>
            </CardHeader>
            <CardContent>
                <Form method="POST">
                    <FieldGroup>
                        <Field>
                            <FieldLabel htmlFor="name">Username</FieldLabel>
                            <Input id="username" name="username" type="text" placeholder="e.g. Testoviron" required/>
                        </Field>
                        <Field>
                            <FieldLabel htmlFor="email">Email</FieldLabel>
                            <Input
                                id="email"
                                name="email"
                                type="email"
                                placeholder="m@example.com"
                            />
                        </Field>
                        <Field>
                            <FieldLabel htmlFor="password">Password</FieldLabel>
                            <Input id="password" name="password" type="password" required/>
                            <FieldDescription>
                                Must be at least 12 characters long.
                            </FieldDescription>
                        </Field>
                        <Field>
                            <FieldLabel htmlFor="confirm-password">
                                Confirm Password
                            </FieldLabel>
                            <Input id="confirm-password" name="confirm-password" type="password" required/>
                        </Field>
                        <FieldGroup>
                            <Field>
                                <Button type="submit" disabled={isSubmitting}>{isSubmitting ? "Creating account..." : "Create Account"}</Button>
                                {error && (
                                    <FieldDescription className="text-center text-red-500 font-medium mt-2">
                                        {error}
                                    </FieldDescription>
                                )}
                            </Field>
                        </FieldGroup>
                    </FieldGroup>
                </Form>
            </CardContent>
        </Card>
    )
}
